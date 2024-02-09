codeunit 50101 "CAR Helper Library"
{
    SingleInstance = true;

    var
        C_UNKNOWN: Label 'UNKNOWN';
        TenantG: Guid;

    procedure Tenant(): Guid
    begin
        exit(TenantG);
    end;

    procedure Tenant(TenantP: Guid)
    begin
        TenantG := TenantP;
    end;

    procedure GuidToJson(GuidP: Guid): Text
    var
        GuidL: Text;
    begin
        GuidL := GuidP;
        GuidL := LowerCase(GuidL);
        GuidL := CopyStr(GuidL, 2);
        GuidL := CopyStr(GuidL, 1, StrLen(GuidL) - 1);
        exit(GuidL);
    end;

    procedure GetText(JObjectP: JsonObject; PathP: Text; var TextP: Text): Boolean
    var
        TokenL: JsonToken;
    begin
        if not JObjectP.Get(PathP, TokenL) then
            exit(false);
        if TokenL.AsValue().IsNull() or TokenL.AsValue().IsUndefined() then
            exit(false);

        TextP := TokenL.AsValue().AsText();
        exit(true);
    end;

    procedure GetCurrencyCode(CurrencyIdP: Guid): Code[10]
    var
        CurrencyL: Record Currency;
        GenLedgSetupL: Record "General Ledger Setup";
    begin
        CurrencyL.Reset();
        CurrencyL.SetRange(SystemId, CurrencyIdP);
        if CurrencyL.FindFirst() then
            exit(CurrencyL.Code);
        if not GenLedgSetupL.Get() then
            exit('');
        exit(GetUnknownLabel());
    end;

    procedure GetUnknownLabel(): Text
    begin
        exit(C_UNKNOWN);
    end;

    procedure GetClusterId(var SetupP: Record "CAR Integration Setup")
    var
        TempBlobL: Codeunit "Temp Blob";
        JsonL: JsonObject;
        TokenL: JsonToken;
        HttpClientL: HttpClient;
        ResponseL: HttpResponseMessage;
        InStreamL: InStream;
    begin
        if not SetupP.Enabled then
            exit;
        SetupP.TestField("Base URI");

        HttpClientL.Get(StrSubstNo('%1/v3/clusters', SetupP."Base URI"), ResponseL);
        InStreamL := TempBlobL.CreateInStream();
        ResponseL.Content.ReadAs(InStreamL);
        JsonL.ReadFrom(InStreamL);

        JsonL.SelectToken('$.data[0].cluster_id', TokenL);
        SetupP."Cluster Id" := TokenL.AsValue().AsText();
    end;

    procedure CreateSalesDocumentJson(SalesHdrP: Record "Sales Header"): JsonObject
    var
        JSalesHeaderL: JsonObject;
    begin
        JSalesHeaderL.Add('OrderId', GuidToJson(SalesHdrP.SystemId));
        JSalesHeaderL.Add('OrderNumber', SalesHdrP."No.");
        JSalesHeaderL.Add('QuoteId', GuidToJson(SalesHdrP."CAR Quote Id"));
        exit(JSalesHeaderL);
    end;

    procedure CreateSalesDocumentJson(SalesInvHdrP: Record "Sales Invoice Header"): JsonObject
    var
        SalesInvLineL: Record "Sales Invoice Line";
        CurrencyL: Record Currency;
        ItemL: Record Item;
        CustomerL: Record Customer;
        VatBusPostingGrpL: Record "VAT Business Posting Group";
        GenBusPostingGrpL: Record "Gen. Business Posting Group";
        VatProdPostingGrpL: Record "VAT Product Posting Group";
        GenProdPostingGrpL: Record "Gen. Product Posting Group";
        ShipMethodL: Record "Shipment Method";
        PayMethodL: Record "Payment Method";
        PayTermsL: Record "Payment Terms";
        LocationL: Record Location;
        BrandL: Record "CAR Brand";
        Base64ConvL: Codeunit "Base64 Convert";
        TempBlobL: Codeunit "Temp Blob";
        JVehicleL: JsonObject;
        JSalesHeaderL: JsonObject;
        JSalesLinesL: JsonObject;
        JSalesLinesPriceL: JsonObject;
        JSalesLinesVATL: JsonObject;
        JSalesLinesArrayL: JsonArray;
        EnumValueNameL: Text;
        DocTypeTextL: Text;
        InStreamL: InStream;
        OutStreamL: OutStream;
    begin
        CustomerL.Get(SalesInvHdrP."Sell-to Customer No.");
        ItemL.Get(SalesInvHdrP."CAR Model Code");
        DocTypeTextL := SalesInvHdrP."CAR Document Type".Names.Get(
            SalesInvHdrP."CAR Document Type".Ordinals.IndexOf(
                SalesInvHdrP."CAR Document Type".AsInteger()));
        if CurrencyL.Get(SalesInvHdrP."Currency Code") then
            ;
        if VatBusPostingGrpL.Get(SalesInvHdrP."VAT Bus. Posting Group") then
            ;
        if GenBusPostingGrpL.Get(SalesInvHdrP."Gen. Bus. Posting Group") then
            ;
        if LocationL.Get(SalesInvHdrP."Location Code") then
            ;
        if ShipMethodL.Get(SalesInvHdrP."Shipment Method Code") then
            ;
        if PayMethodL.Get(SalesInvHdrP."Payment Method Code") then
            ;
        if PayTermsL.Get(SalesInvHdrP."Payment Terms Code") then
            ;
        if BrandL.Get(SalesInvHdrP."CAR Brand") then
            ;
        JSalesHeaderL.Add('Id', SalesInvHdrP."CAR Posted Document Id");
        JSalesHeaderL.Add('TransactionId', SalesInvHdrP.SystemId);
        JSalesHeaderL.Add('OrderId', GuidToJson(SalesInvHdrP."CAR Order Id"));
        JSalesHeaderL.Add('OrderNo', SalesInvHdrP."Order No.");
        JSalesHeaderL.Add('DocumentType', DocTypeTextL);
        JSalesHeaderL.Add('DocumentNo', SalesInvHdrP."No.");
        JSalesHeaderL.Add('Date', SalesInvHdrP."Posting Date");
        JSalesHeaderL.Add('CustomerType', Format(CustomerL."Partner Type", 0, 1));
        JSalesHeaderL.Add('BusinessPartnerId', CustomerL.SystemId);
        JSalesHeaderL.Add('CustomerNo', CustomerL."No.");
        JSalesHeaderL.Add('CustomerName', CustomerL.Name);
        JSalesHeaderL.Add('CurrencyId', CurrencyL.SystemId);
        JSalesHeaderL.Add('TotalAmount', SalesInvHdrP.Amount);
        JSalesHeaderL.Add('TotalAmountIncludingTax', SalesInvHdrP."Amount Including VAT");

        JSalesHeaderL.Add('BusinessTaxGroupId', VatBusPostingGrpL.SystemId);
        JSalesHeaderL.Add('BusinessPartnerGroupId', GenBusPostingGrpL.SystemId);

        JSalesHeaderL.Add('BranchId', LocationL.SystemId);
        JSalesHeaderL.Add('ShipmentMethod', ShipMethodL.SystemId);
        JSalesHeaderL.Add('PaymentMethod', PayMethodL.SystemId);
        JSalesHeaderL.Add('PaymentTerms', PayTermsL.SystemId);
        JSalesHeaderL.Add('DueDate', SalesInvHdrP."Due Date");
        JSalesHeaderL.Add('PricesIncludingTax', SalesInvHdrP."Amount Including VAT");
        JSalesHeaderL.Add('Status', 'Posted');
        JSalesHeaderL.Add('ReferenceId', SalesInvHdrP."External Document No.");
        JSalesHeaderL.Add('Correction', SalesInvHdrP.Correction);
        TempBlobL.CreateInStream(InStreamL, TextEncoding::UTF8);
        TempBlobL.CreateOutStream(OutStreamL, TextEncoding::UTF8);
        SalesInvHdrP."CAR Invoice Pdf".ExportStream(OutStreamL);
        JSalesHeaderL.Add('Pdf', Base64ConvL.ToBase64(InStreamL));
        JSalesHeaderL.Add('Vehicle', JVehicleL);
        JVehicleL.Add('Id', GuidToJson(CreateGuid()));
        JVehicleL.Add('Vin', SalesInvHdrP."CAR VIN");
        JVehicleL.Add('BrandId', GuidToJson(BrandL.SystemId));
        JVehicleL.Add('RegistrationDate', SalesInvHdrP."CAR Registration Date");
        JVehicleL.Add('LicenseNo', SalesInvHdrP."CAR License No.");
        JVehicleL.Add('ModelId', GuidToJson(ItemL.SystemId));
        JVehicleL.Add('MileageUnitOfMeasure', 'Kilometer');
        JVehicleL.Add('BusinessPartnerType', 'Customer');
        JVehicleL.Add('VehicleStatus', 'Customer');
        JVehicleL.Add('VehicleType', 'Car');
        JVehicleL.Add('ExternalId', SalesInvHdrP."CAR VIN");
        JVehicleL.Add('Code', SalesInvHdrP."CAR VIN");
        JVehicleL.Add('BusinessPartnerId', GuidToJson(CustomerL.SystemId));
        JSalesHeaderL.Add('Lines', JSalesLinesArrayL);

        SalesInvLineL.SetRange("Document No.", SalesInvHdrP."No.");
        SalesInvLineL.FindSet();
        repeat
            Clear(JSalesLinesL);
            Clear(JSalesLinesPriceL);
            Clear(JSalesLinesVATL);
            ItemL.SetRange("No.", SalesInvLineL."No.");
            if ItemL.FindFirst() then begin
                EnumValueNameL := ItemL.Type.Names.Get(ItemL.Type.Ordinals.IndexOf(ItemL.Type.AsInteger()));
                if ItemL.Type <> ItemL.Type::Service then
                    JSalesLinesL.Add('ProductType', 'Item')
                else
                    JSalesLinesL.Add('ProductType', EnumValueNameL);
            end;

            JSalesLinesL.Add('ProductId', GuidToJson(ItemL.SystemId));
            JSalesLinesL.Add('Description', SalesInvLineL.Description);
            if VatProdPostingGrpL.Get(SalesInvLineL."VAT Prod. Posting Group") then
                JSalesLinesL.Add('ProductTaxGroupId', SalesInvLineL."VAT Prod. Posting Group");
            if GenProdPostingGrpL.Get(SalesInvLineL."Gen. Prod. Posting Group") then
                JSalesLinesL.Add('ProductGroupId', GenProdPostingGrpL.SystemId);

            JSalesLinesL.Add('Quantity', SalesInvLineL.Quantity);
            JSalesLinesPriceL.Add('UnitPrice', SalesInvLineL."Unit Price");
            JSalesLinesPriceL.Add('DiscountPercentage', SalesInvLineL."Line Discount %");
            JSalesLinesPriceL.Add('DiscountAmount', SalesInvLineL."Line Discount Amount");
            JSalesLinesPriceL.Add('Amount', SalesInvLineL.Amount);
            JSalesLinesL.Add('Price', JSalesLinesPriceL);

            JSalesLinesVATL.Add('TaxCode', SalesInvLineL."VAT Identifier");
            JSalesLinesVATL.Add('VatPercentage', SalesInvLineL."VAT %");
            JSalesLinesL.Add('Vat', JSalesLinesVATL);
            JSalesLinesArrayL.Add(JSalesLinesL);
        until SalesInvLineL.Next() = 0;
        exit(JSalesHeaderL);
    end;
}
