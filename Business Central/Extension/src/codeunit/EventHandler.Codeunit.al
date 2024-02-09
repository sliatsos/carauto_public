codeunit 50106 "CAR Event Handler"
{
    SingleInstance = true;

    var
        SalesBlockedErr: Label 'You cannot sell this item because the Sales Blocked check box is selected on the item card.';


    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Company Triggers", 'OnCompanyOpenCompleted', '', true, false)]
    local procedure OnCompanyOpenCompletedSub()
    var
        TenantL: Record "CAR Tenant";
        TenantsL: Page "CAR Tenants";
        HelperL: Codeunit "CAR Helper Library";
    begin
        if TenantL.IsEmpty() then
            exit;
        if Session.GetModuleExecutionContext() <> ExecutionContext::Normal then
            exit;

        TenantsL.LookupMode := true;
        if TenantsL.RunModal() <> Action::LookupOK then
            TenantsL.GetRecord(TenantL)
        else
            TenantL.FindFirst();
        HelperL.Tenant(TenantL.Id);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Option Lookup Buffer", 'OnBeforeIncludeOption', '', false, false)]
    local procedure OnBeforeIncludeOption(OptionLookupBuffer: Record "Option Lookup Buffer" temporary; LookupType: Option; Option: Integer; var Handled: Boolean; var Result: Boolean; RecRef: RecordRef);
    begin

        if (LookupType <> 0) then begin
            Handled := false;
            exit;
        end;

        if (Option < 50100) then begin
            Handled := false;
            exit;
        end;

        Handled := true;
        Result := true;
    end;

    [EventSubscriber(ObjectType::Table, Database::"Sales Line", 'OnAfterAssignFieldsForNo', '', false, false)]
    local procedure OnAfterAssignFieldsForNoSub(var SalesLine: Record "Sales Line"; SalesHeader: Record "Sales Header")
    begin
        CopyFromItem(SalesLine, SalesHeader);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Sales-Post", 'OnAfterPostSalesDoc', '', false, false)]
    local procedure OnAfterPostSalesDocSub(CommitIsSuppressed: Boolean; SalesInvHdrNo: Code[20]; var SalesHeader: Record "Sales Header")
    var
        SalesInvHdrL: Record "Sales Invoice Header";
    begin
        if CommitIsSuppressed then
            exit;

        if SalesInvHdrNo = '' then
            exit;

        SalesInvHdrL.Get(SalesInvHdrNo);
        if not SalesInvHdrL."CAR Is Vehicle Order" then
            exit;

        SalesInvHdrL."CAR Kafka Status" := SalesInvHdrL."CAR Kafka Status"::Posted;
        SalesInvHdrL."CAR Order Id" := SalesHeader.SystemId;
        SalesInvHdrL.Modify();
        Commit();

        SaveReportAsPDF(SalesInvHdrL);
        Codeunit.Run(Codeunit::"CAR Kafka Sender");
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Sales-Post", 'OnBeforePostSalesDoc', '', false, false)]
    local procedure OnBeforePostSalesDocSub(CommitIsSuppressed: Boolean; PreviewMode: Boolean; var SalesHeader: Record "Sales Header")
    begin
        if CommitIsSuppressed then
            exit;

        if not SalesHeader."CAR Is Vehicle Order" then
            exit;

        if SalesHeader."CAR VIN" = '' then
            Error('%1 must be filled before posting.', SalesHeader.FieldCaption("CAR VIN"));
        if SalesHeader."CAR License No." = '' then
            Error('%1 must be filled before posting.', SalesHeader.FieldCaption("CAR License No."));
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Sales-Quote to Order (Yes/No)", 'OnAfterSalesQuoteToOrderRun', '', false, false)]
    local procedure OnAfterSalesQuoteToOrderRunSub(var SalesHeader: Record "Sales Header"; var SalesHeader2: Record "Sales Header")
    begin
        SalesHeader2."CAR Quote Id" := SalesHeader.SystemId;
        SalesHeader2."CAR Kafka Status" := "CAR Kafka Status"::OrderCreated;
        SalesHeader2.Modify();
        Commit();
        Codeunit.Run(Codeunit::"CAR Kafka Sender");
    end;

    local procedure CopyFromItem(var SalesLineP: Record "Sales Line"; SalesHeaderP: Record "Sales Header")
    var
        ItemL: Record Item;
        PrepaymentMgtL: Codeunit "Prepayment Mgt.";
        PostingSetupMgtL: Codeunit PostingSetupManagement;
        UOMMgtL: Codeunit "Unit of Measure Management";
    begin
        SalesLineP.GetItem(ItemL);

        ItemL.TestField(Blocked, false);
        ItemL.TestField("Gen. Prod. Posting Group");
        if ItemL."Sales Blocked" then
            Error(SalesBlockedErr);
        if ItemL.Type = ItemL.Type::Inventory then begin
            ItemL.TestField("Inventory Posting Group");
            SalesLineP."Posting Group" := ItemL."Inventory Posting Group";
        end;

        SalesLineP.Description := ItemL.Description;
        SalesLineP."Description 2" := ItemL."Description 2";
        if ItemL."Sales Unit of Measure" <> '' then
            SalesLineP."Unit of Measure Code" := ItemL."Sales Unit of Measure"
        else
            SalesLineP."Unit of Measure Code" := ItemL."Base Unit of Measure";
        SalesLineP."Qty. per Unit of Measure" := UOMMgtL.GetQtyPerUnitOfMeasure(ItemL, SalesLineP."Unit of Measure Code");
        SalesLineP.ValidateUnitCostLCYOnGetUnitCost(ItemL);
        SalesLineP."Allow Invoice Disc." := ItemL."Allow Invoice Disc.";
        SalesLineP."Units per Parcel" := ItemL."Units per Parcel";
        SalesLineP."Gen. Prod. Posting Group" := ItemL."Gen. Prod. Posting Group";
        SalesLineP."VAT Prod. Posting Group" := ItemL."VAT Prod. Posting Group";
        SalesLineP."Tax Group Code" := ItemL."Tax Group Code";
        SalesLineP."Package Tracking No." := SalesHeaderP."Package Tracking No.";
        SalesLineP."Item Category Code" := ItemL."Item Category Code";
        SalesLineP.Nonstock := ItemL."Created From Nonstock Item";
        SalesLineP."Profit %" := ItemL."Profit %";
        SalesLineP."Allow Item Charge Assignment" := true;
        PrepaymentMgtL.SetSalesPrepaymentPct(SalesLineP, SalesHeaderP."Posting Date");
        if SalesLineP.IsInventoriableItem() then
            PostingSetupMgtL.CheckInvtPostingSetupInventoryAccount(SalesLineP."Location Code", SalesLineP."Posting Group");

        if SalesHeaderP."Language Code" <> '' then
            SalesLineP.GetItemTranslation();

        if ItemL.Reserve = ItemL.Reserve::Optional then
            SalesLineP.Reserve := SalesHeaderP.Reserve
        else
            SalesLineP.Reserve := ItemL.Reserve;
    end;

    local procedure SaveReportAsPDF(SalesInvHdrP: Record "Sales Invoice Header")
    var
        TempBlobL: Codeunit "Temp Blob";
        OutStreamL: OutStream;
        InStreamL: InStream;
        RecRefL: RecordRef;
        SalesInvHdrL: Record "Sales Invoice Header";
    begin
        TempBlobL.CreateOutStream(OutStreamL, TextEncoding::UTF8);
        TempBlobL.CreateInStream(InStreamL, TextEncoding::UTF8);
        SalesInvHdrL.Reset();
        SalesInvHdrL.SetRange("No.", SalesInvHdrP."No.");
        SalesInvHdrL.FindFirst();
        RecRefL.GetTable(SalesInvHdrL);
        Report.SaveAs(Report::"Standard Sales - Invoice", '', ReportFormat::Pdf, OutStreamL, RecRefL);
        SalesInvHdrL.Get(SalesInvHdrL."No.");
        SalesInvHdrL."CAR Invoice Pdf".ImportStream(InStreamL, STRSUBSTNO('SalesInvoice_%1.Pdf', SalesInvHdrP."No."));
        SalesInvHdrL.Modify();
        Commit();
    end;

}
