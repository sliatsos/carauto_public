codeunit 50104 "CAR Order Json Parser"
{
    var
        HelperG: Codeunit "CAR Helper Library";

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CAR Order Kafka Receiver", 'OnKafkaMessageReceivedEvent', '', true, false)]
    local procedure OnKafkaMessageReceivedSub(var TempBlobP: Codeunit "Temp Blob")
    var
        InStreamL: InStream;
        JOrderL: JsonObject;
    begin
        TempBlobP.CreateInStream(InStreamL);
        JOrderL.ReadFrom(InStreamL);
        ParseSalesOrderJson(JOrderL);
    end;

    local procedure ParseSalesOrderJson(JSalesOrderP: JsonObject)
    var
        SalesHdrL: Record "Sales Header";
        SalesLineL: Record "Sales Line";
        JValueL: JsonToken;
        JLinesArrayL: JsonArray;
        KafkaKeyL: Text;
    begin
        // Header
        if JSalesOrderP.Get('KafkaKey', JValueL) then
            KafkaKeyL := JValueL.AsValue().AsText();

        SalesHdrL.SetRange(SystemId, KafkaKeyL);
        if SalesHdrL.FindFirst() then
            SalesHdrL.Delete(true);
        SalesHdrL.SetRange(SystemId);

        if not FillNextSalesHeaderFromJson(JSalesOrderP, SalesHdrL, KafkaKeyL) then
            exit;
        SalesHdrL.Modify();

        // Lines
        if JSalesOrderP.Get('Options', JValueL) then begin
            JLinesArrayL := JValueL.AsArray();
            FillNextSalesLineFromJson(JLinesArrayL, SalesHdrL, SalesLineL);
        end;
    end;

    local procedure FillNextSalesHeaderFromJson(JSalesOrderP: JsonObject; var SalesHdrP: Record "Sales Header"; KafkaKeyP: Text): Boolean
    var
        GenLedgerSetupL: Record "General Ledger Setup";
        SalesDocIntegrSetupL: Record "CAR Integration Setup";
        ModelL: Record Item;
        LocationL: Record Location;
        CustL: Record Customer;
        JValueL: JsonToken;
    begin
        GenLedgerSetupL.Get();
        SalesDocIntegrSetupL.Get();
        SalesHdrP.Init();
        if JSalesOrderP.Get('OrderNumber', JValueL) then
            SalesHdrP.Validate("No.", JValueL.AsValue().AsCode());

        SalesHdrP.Validate("Document Type", SalesHdrP."Document Type"::Quote);
        SalesHdrP.SystemId := KafkaKeyP;
        SalesHdrP.Insert(true, true);

        if JSalesOrderP.Get('ModelId', JValueL) then
            if ModelL.GetBySystemId(JValueL.AsValue().AsText()) then begin
                SalesHdrP."CAR Brand" := ModelL."CAR Brand Code";
                SalesHdrP."CAR Model Code" := ModelL."No.";
                SalesHdrP."CAR Model Year" := ModelL."CAR Model Year";
                SalesHdrP."CAR Model Description" := ModelL.Description;
            end;

        SalesHdrP."CAR Registration Date" := Today();
        SalesHdrP."CAR Document Type" := "CAR Document Type"::Vehicle;
        SalesHdrP."CAR Is Vehicle Order" := true;

        if JSalesOrderP.Get('Date', JValueL) then
            SalesHdrP."Posting Date" := DT2Date(JValueL.AsValue().AsDateTime());

        if JSalesOrderP.Get('CustomerId', JValueL) then
            if CustL.GetBySystemId(JValueL.AsValue().AsText()) then
                SalesHdrP.Validate("Sell-to Customer No.", CustL."No.");

        if JSalesOrderP.Get('CurrencyId', JValueL) then
            SalesHdrP.Validate("Currency Code", HelperG.GetCurrencyCode(JValueL.AsValue().AsText()));

        if JSalesOrderP.Get('BranchId', JValueL) then
            if LocationL.GetBySystemId(JValueL.AsValue().AsText()) then
                SalesHdrP.Validate("Location Code", LocationL.Code);

        //SalesHdrP."CAR Tenant Id" := TenantIdL;

        if JSalesOrderP.Get('DueDate', JValueL) then
            if not JValueL.AsValue().IsNull() then
                SalesHdrP."Due Date" := DT2Date(JValueL.AsValue().AsDateTime());

        if JSalesOrderP.Get('PricesIncludingTax', JValueL) then
            SalesHdrP."Prices Including VAT" := JValueL.AsValue().AsBoolean();

        SalesHdrP."CAR Kafka Status" := "CAR Kafka Status"::Created;

        if JSalesOrderP.Get('ReferenceId', JValueL) then
            SalesHdrP."External Document No." := JValueL.AsValue().AsText();

        if JSalesOrderP.Get('Correction', JValueL) then
            SalesHdrP.Correction := JValueL.AsValue().AsBoolean();

        if JSalesOrderP.Get('Vehicle', JValueL) then
            if JValueL.IsObject() then
                FillNextVehicleFromJson(SalesHdrP, JValueL.AsObject());

        SalesHdrP.Modify(true);
        exit(true);
    end;

    local procedure FillNextVehicleFromJson(var SalesHdrP: Record "Sales Header"; JObjectP: JsonObject)
    var
        JTokenL: JsonToken;
        JModelL: JsonObject;
        JBrandL: JsonObject;
    begin
        if JObjectP.Get('Vin', JTokenL) then
            SalesHdrP."CAR VIN" := JTokenL.AsValue().AsText();

        if JObjectP.Get('LicenseNo', JTokenL) then
            SalesHdrP."CAR License No." := CopyStr(JTokenL.AsValue().AsText(), 1, MaxStrLen(SalesHdrP."CAR License No."));

        if JObjectP.Get('RegistrationDate', JTokenL) then
            SalesHdrP."CAR Registration Date" := DT2Date(JTokenL.AsValue().AsDateTime());

        if JObjectP.Get('Model', JTokenL) then
            if JTokenL.IsObject() then begin
                JModelL := JTokenL.AsObject();

                if JModelL.Get('Code', JTokenL) then
                    SalesHdrP."CAR Model Code" := CopyStr(JTokenL.AsValue().AsCode(), 1, MaxStrLen(SalesHdrP."CAR Model Code"));

                if JModelL.Get('Description', JTokenL) then
                    SalesHdrP."CAR Model Description" := CopyStr(JTokenL.AsValue().AsText(), 1, MaxStrLen(SalesHdrP."CAR Model Code"));

                if JModelL.Get('ModelYear', JTokenL) then
                    SalesHdrP."CAR Model Year" := JTokenL.AsValue().AsInteger();
            end;

        if JObjectP.Get('Brand', JTokenL) then
            if JTokenL.IsObject() then begin
                JBrandL := JTokenL.AsObject();

                if JBrandL.Get('Code', JTokenL) then
                    SalesHdrP."CAR Brand" := CopyStr(JTokenL.AsValue().AsCode(), 1, MaxStrLen(SalesHdrP."CAR Brand"));
            end;
    end;

    local procedure FillNextSalesLineFromJson(JSalesLinesP: JsonArray; SalesHdrP: record "Sales Header"; var SalesLineP: Record "Sales Line")
    var
        ItemL: Record Item;
        LineNoL: Integer;
        JSalesLineTokenL: JsonToken;
        JSalesLineL: JsonObject;
        JTokenL: JsonToken;
        JObjL: JsonObject;
        JValueL: JsonToken;
    begin
        if JSalesLinesP.Count() = 0 then
            exit;

        foreach JsalesLineTokenL in JSalesLinesP do begin
            JSalesLineL := JSalesLineTokenL.AsObject();
            SalesLineP.Init();
            LineNoL += 10000;
            SalesLineP."Document No." := SalesHdrP."No.";
            SalesLineP."Document Type" := SalesHdrP."Document Type";
            SalesLineP."Line No." := LineNoL;
            SalesLineP.Insert(true);
            SalesLineP.Validate(Type, SalesLineP.Type::"Vehicle Option");
            if JSalesLineL.Get('Code', JValueL) then
                if ItemL.Get(JValueL.AsValue().AsCode()) then
                    SalesLineP.Validate("No.", ItemL."No.");
            if JSalesLineL.Get('Description', JValueL) then
                SalesLineP.Description := JValueL.AsValue().AsText();

            if JSalesLineL.Get('Quantity', JValueL) then
                SalesLineP.Validate(Quantity, JValueL.AsValue().AsDecimal())
            else
                SalesLineP.Validate(Quantity, 1);

            SalesLineP."CAR Brand" := SalesHdrP."CAR Brand";
            if JSalesLineL.Get('UnitPrice', JValueL) then
                SalesLineP.Validate("Unit Price", JValueL.AsValue().AsDecimal());
            if JSalesLineL.Get('UnitCost', JValueL) then
                SalesLineP.Validate("Unit Cost", JValueL.AsValue().AsDecimal());
            if JSalesLineL.Get('DiscountPercentage', JValueL) then
                SalesLineP.Validate("Line Discount %", JValueL.AsValue().AsDecimal());
            if JSalesLineL.Get('DiscountAmount', JValueL) then
                SalesLineP.Validate("Line Discount Amount", JValueL.AsValue().AsDecimal());

            if JSalesLineL.Get('Vat', JTokenL) then begin
                JObjL := JTokenL.AsObject();
                if JObjL.Get('TaxCode', JValueL) then
                    SalesLineP."VAT Identifier" := JValueL.AsValue().AsText();
                if JObjL.Get('VatPercentage', JValueL) then
                    SalesLineP.Validate("VAT %", JValueL.AsValue().AsDecimal());
            end;

            SalesLineP.Modify();
            //Line dimensions for the moment contains copy of the header dimensions and 
            //default item dimension. These are created on the validation of No.
        end;
    end;

    [TryFunction]
    local procedure TryGetObject(JValueP: JsonToken; var JObjectP: JsonObject)
    begin
        if JValueP.IsValue() then
            Error('');
        JObjectP := JValueP.AsObject();
    end;

    local procedure GetDimValue(DimSetIdP: Integer; DimSetCodeP: Code[20]): Code[20]
    var
        DimSetEntryL: Record "Dimension Set Entry";
    begin
        if DimSetEntryL.Get(DimSetIdP, DimSetCodeP) then
            exit(DimSetEntryL."Dimension Value Code");
    end;
}
