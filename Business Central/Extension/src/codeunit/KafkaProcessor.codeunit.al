codeunit 50107 "CAR KafkaProcessor"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CAR Kafka Receiver", 'OnKafkaMessageReceivedEvent', '', false, false)]
    local procedure OnKafkaMessageReceivedEvent(var TempBlobP: Codeunit "Temp Blob");
    var
        InStreamL: InStream;
        JDocL: JsonObject;
        ActionL: Enum "CAR Actions";
        TokenL: JsonToken;
        EntityL: Text;
    begin
        TempBlobP.CreateInStream(InStreamL);
        JDocL.ReadFrom(InStreamL);

        if JDocL.Get('Action', TokenL) then
            ActionL := ActionL.Names.IndexOf(TokenL.AsValue().AsText()) - 1;
        if JDocL.Get('Entity', TokenL) then
            EntityL := TokenL.AsValue().AsText();
        if JDocL.Get('Payload', TokenL) then
            if TokenL.AsObject().Get(EntityL, TokenL) then
                if TokenL.AsArray().Get(0, TokenL) then
                    ProcessRecord(EntityL, TokenL.AsObject(), ActionL);
    end;

    local procedure ProcessRecord(EntityP: Text; ObjectP: JsonObject; ActionP: Enum "CAR Actions")
    begin
        case EntityP of
            'Customer':
                ProcessCustomer(ObjectP, ActionP);
            'Salesperson':
                ProcessSalesPerson(ObjectP, ActionP);
            'Brands':
                ProcessBrand(ObjectP, ActionP);
            'Models':
                ProcessItem(ObjectP, ActionP, "Item Type"::Model);
            'Options':
                ProcessItem(ObjectP, ActionP, "Item Type"::Option);
        end;
    end;

    local procedure ProcessItem(JObjectP: JsonObject; ActionP: Enum "CAR Actions"; ItemTypeP: Enum "Item Type")
    var
        ModelL: Record Item;
        BrandL: Record "CAR Brand";
        ItemTemplL: Record "Item Templ.";
        Base64ConvL: Codeunit "Base64 Convert";
        HelperL: Codeunit "CAR Helper Library";
        TempBlobL: Codeunit "Temp Blob";
        ItemTemplMgtL: Codeunit "Item Templ. Mgt.";
        TokenL: JsonToken;
        RecordExistsL: Boolean;
        IdL: Guid;
        TextL: Text;
        OutStreamL: OutStream;
        InStreamL: InStream;
    begin
        JObjectP.Get('Id', TokenL);
        IdL := TokenL.AsValue().AsText();
        RecordExistsL := ModelL.GetBySystemId(IdL);

        if ActionP = ActionP::Delete then begin
            ModelL.Delete(true);
            exit;
        end;

        if not RecordExistsL then begin
            JObjectP.Get('Code', TokenL);
            ItemTemplL.SetRange(Type, ItemTemplL.Type::Inventory);
            ItemTemplL.FindFirst();
            ModelL."No." := TokenL.AsValue().AsCode();
            ModelL.SystemId := IdL;
            ModelL.Insert(true, true);
            ItemTemplMgtL.ApplyItemTemplate(ModelL, ItemTemplL);
        end;
        ModelL.Type := ItemTypeP;
        if HelperL.GetText(JObjectP, 'Description', TextL) then
            ModelL.Validate(Description, TextL);
        if JObjectP.Get('ModelYear', TokenL) then
            ModelL.Validate("CAR Model Year", TokenL.AsValue().AsInteger());

        if HelperL.GetText(JObjectP, 'BrandId', TextL) then
            if BrandL.GetBySystemId(TextL) then
                ModelL.Validate("CAR Brand Code", BrandL.Code);

        if HelperL.GetText(JObjectP, 'InternalCode', TextL) then
            ModelL.Validate("CAR Internal Code", TextL);
        if HelperL.GetText(JObjectP, 'Type', TextL) then
            Evaluate(ModelL."CAR Type", TextL);
        if HelperL.GetText(JObjectP, 'UnitPrice', TextL) then
            Evaluate(ModelL."Unit Price", TextL);
        if HelperL.GetText(JObjectP, 'UnitCost', TextL) then
            Evaluate(ModelL."Unit Cost", TextL);
        if HelperL.GetText(JObjectP, 'Image', TextL) then
            if TextL <> '' then begin
                TempBlobL.CreateOutStream(OutStreamL);
                TempBlobL.CreateInStream(InStreamL);
                Base64ConvL.FromBase64(TextL, OutStreamL);
                ModelL.Picture.ImportStream(InStreamL, '');
            end;

        ModelL.Modify(true);
    end;

    local procedure ProcessBrand(JObjectP: JsonObject; ActionP: Enum "CAR Actions")
    var
        BrandL: Record "CAR Brand";
        Base64ConvL: Codeunit "Base64 Convert";
        HelperL: Codeunit "CAR Helper Library";
        TempBlobL: Codeunit "Temp Blob";
        TokenL: JsonToken;
        RecordExistsL: Boolean;
        IdL: Guid;
        TextL: Text;
        OutStreamL: OutStream;
        InStreamL: InStream;
    begin
        JObjectP.Get('Id', TokenL);
        IdL := TokenL.AsValue().AsText();
        RecordExistsL := BrandL.GetBySystemId(IdL);

        if ActionP = ActionP::Delete then begin
            BrandL.Delete(true);
            exit;
        end;

        if not RecordExistsL then begin
            JObjectP.Get('Code', TokenL);
            BrandL.Code := TokenL.AsValue().AsCode();
            BrandL.SystemId := IdL;
            BrandL.Insert(true, true);
        end;

        if HelperL.GetText(JObjectP, 'DisplayName', TextL) then
            BrandL.Validate(Description, TextL);
        if HelperL.GetText(JObjectP, 'Image', TextL) then
            if TextL <> '' then begin
                TempBlobL.CreateOutStream(OutStreamL);
                TempBlobL.CreateInStream(InStreamL);
                Base64ConvL.FromBase64(TextL, OutStreamL);
                BrandL.Image.ImportStream(InStreamL, '');
            end;

        BrandL.Modify(true);
    end;

    local procedure ProcessSalesPerson(JObjectP: JsonObject; ActionP: Enum "CAR Actions")
    var
        SalespersonL: Record "Salesperson/Purchaser";
        Base64ConvL: Codeunit "Base64 Convert";
        HelperL: Codeunit "CAR Helper Library";
        TempBlobL: Codeunit "Temp Blob";
        TokenL: JsonToken;
        RecordExistsL: Boolean;
        IdL: Guid;
        TextL: Text;
        OutStreamL: OutStream;
        InStreamL: InStream;
    begin
        JObjectP.Get('Id', TokenL);
        IdL := TokenL.AsValue().AsText();
        RecordExistsL := SalespersonL.GetBySystemId(IdL);

        if ActionP = ActionP::Delete then begin
            SalespersonL.Delete(true);
            exit;
        end;

        if not RecordExistsL then begin
            JObjectP.Get('Code', TokenL);
            SalespersonL.Code := TokenL.AsValue().AsCode();
            SalespersonL.SystemId := IdL;
            SalespersonL.Insert(true, true);
        end;

        if HelperL.GetText(JObjectP, 'DisplayName', TextL) then
            SalespersonL.Validate(Name, TextL);
        if HelperL.GetText(JObjectP, 'Image', TextL) then
            if TextL <> '' then begin
                TempBlobL.CreateOutStream(OutStreamL);
                TempBlobL.CreateInStream(InStreamL);
                Base64ConvL.FromBase64(TextL, OutStreamL);
                SalespersonL.Image.ImportStream(InStreamL, '');
            end;
        if HelperL.GetText(JObjectP, 'Email', TextL) then
            SalespersonL.Validate("E-Mail", TextL);
        if HelperL.GetText(JObjectP, 'Phone', TextL) then
            SalespersonL.Validate("Phone No.", TextL);

        SalespersonL.Modify(true);
    end;

    local procedure ProcessCustomer(JObjectP: JsonObject; ActionP: Enum "CAR Actions")
    var
        CustomerL: Record Customer;
        CustomerTemplL: Record "Customer Templ.";
        CustTemplMgtL: Codeunit "Customer Templ. Mgt.";
        Base64ConvL: Codeunit "Base64 Convert";
        HelperL: Codeunit "CAR Helper Library";
        TempBlobL: Codeunit "Temp Blob";
        ContactTypeL: Enum "Contact Type";
        TokenL: JsonToken;
        AddressL: JsonObject;
        CommL: JsonObject;
        RecordExistsL: Boolean;
        IdL: Guid;
        TextL: Text;
        OutStreamL: OutStream;
        InStreamL: InStream;
    begin
        JObjectP.Get('Id', TokenL);
        IdL := TokenL.AsValue().AsText();
        RecordExistsL := CustomerL.GetBySystemId(IdL);

        if ActionP = ActionP::Delete then begin
            CustomerL.Delete(true);
            exit;
        end;

        JObjectP.Get('Type', TokenL);
        Evaluate(ContactTypeL, TokenL.AsValue().AsText());

        if not RecordExistsL then begin
            CustomerTemplL.Reset();
            CustomerTemplL.SetRange("Contact Type", ContactTypeL);
            CustomerTemplL.FindFirst();
            JObjectP.Get('Code', TokenL);
            CustomerL."No." := TokenL.AsValue().AsCode();
            CustomerL.SystemId := IdL;
            CustomerL.Insert(true, true);
            CustTemplMgtL.ApplyCustomerTemplate(CustomerL, CustomerTemplL);
        end;

        if HelperL.GetText(JObjectP, 'DisplayName', TextL) then
            CustomerL.Validate(Name, TextL);
        if HelperL.GetText(JObjectP, 'Image', TextL) then
            if TextL <> '' then begin
                TempBlobL.CreateOutStream(OutStreamL);
                TempBlobL.CreateInStream(InStreamL);
                Base64ConvL.FromBase64(TextL, OutStreamL);
                CustomerL.Image.ImportStream(InStreamL, '');
            end;

        if HelperL.GetText(JObjectP, 'SalespersonCode', TextL) then
            CustomerL.Validate("Salesperson Code", TextL);

        if HelperL.GetText(JObjectP, 'VATRegistrationNo', TextL) then
            CustomerL.Validate("VAT Registration No.", TextL);

        if JObjectP.Get('Address', TokenL) then begin
            AddressL := TokenL.AsObject();
            if HelperL.GetText(AddressL, 'Adress', TextL) then
                CustomerL.Validate(Address, TextL);
            if HelperL.GetText(AddressL, 'City', TextL) then
                CustomerL.Validate(City, TextL);
            if HelperL.GetText(AddressL, 'PostCode', TextL) then
                CustomerL.Validate("Post Code", TextL);
            if HelperL.GetText(AddressL, 'County', TextL) then
                CustomerL.Validate(County, TextL);
            if HelperL.GetText(AddressL, 'Country', TextL) then
                CustomerL.Validate("Country/Region Code", TextL);
        end;

        if JObjectP.Get('Communication', TokenL) then begin
            CommL := TokenL.AsObject();
            if HelperL.GetText(CommL, 'Email', TextL) then
                CustomerL.Validate("E-Mail", TextL);
            if HelperL.GetText(CommL, 'Phone', TextL) then
                CustomerL.Validate("Phone No.", TextL);
            if HelperL.GetText(CommL, 'Mobile', TextL) then
                CustomerL.Validate("Mobile Phone No.", TextL);
        end;
        CustomerL.Modify(true);
    end;
}
