codeunit 50105 "CAR Setup Sync. Subscriber"
{
    SingleInstance = true;

    [EventSubscriber(ObjectType::Table, Database::"VAT Business Posting Group", 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertVATBusPostingGroup(var Rec: Record "VAT Business Posting Group")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Business Posting Group", 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyVATBusPostingGroup(var Rec: Record "VAT Business Posting Group"; var xRec: Record "VAT Business Posting Group")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Business Posting Group", 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameVATBusPostingGroup(var Rec: Record "VAT Business Posting Group"; var xRec: Record "VAT Business Posting Group")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Business Posting Group", 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteVATBusPostingGroup(var Rec: Record "VAT Business Posting Group")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Product Posting Group", 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertVATProdPostingGroup(var Rec: Record "VAT Product Posting Group")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Product Posting Group", 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyVATProdPostingGroup(var Rec: Record "VAT Product Posting Group"; var xRec: Record "VAT Product Posting Group")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Product Posting Group", 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameVATProdPostingGroup(var Rec: Record "VAT Product Posting Group"; var xRec: Record "VAT Product Posting Group")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Product Posting Group", 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteVATProdPostingGroup(var Rec: Record "VAT Product Posting Group")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;


    [EventSubscriber(ObjectType::Table, Database::"VAT Posting Setup", 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertVATPostingSetup(var Rec: Record "VAT Posting Setup")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Posting Setup", 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyVATPostingSetup(var Rec: Record "VAT Posting Setup"; var xRec: Record "VAT Posting Setup")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Posting Setup", 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameVATPostingSetup(var Rec: Record "VAT Posting Setup"; var xRec: Record "VAT Posting Setup")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"VAT Posting Setup", 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteVATPostingSetup(var Rec: Record "VAT Posting Setup")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Company Information", 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertCompanyInformation(var Rec: Record "Company Information")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Company Information", 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyCompanyInformation(var Rec: Record "Company Information"; var xRec: Record "Company Information")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Company Information", 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameCompanyInformation(var Rec: Record "Company Information"; var xRec: Record "Company Information")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Company Information", 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteCompanyInformation(var Rec: Record "Company Information")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    [EventSubscriber(ObjectType::Table, Database::Currency, 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertCurrency(var Rec: Record Currency)
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::Currency, 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyCurrency(var Rec: Record Currency; var xRec: Record Currency)
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::Currency, 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameCurrency(var Rec: Record Currency; var xRec: Record Currency)
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::Currency, 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteCurrency(var Rec: Record Currency)
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertCustomer(var Rec: Record Customer)
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifyCustomer(var Rec: Record Customer; var xRec: Record Customer)
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameCustomer(var Rec: Record Customer; var xRec: Record Customer)
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteCustomer(var Rec: Record Customer)
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Salesperson/Purchaser", 'OnAfterInsertEvent', '', true, false)]
    local procedure OnAfterInsertSalesperson(var Rec: Record "Salesperson/Purchaser")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Insert);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Salesperson/Purchaser", 'OnAfterModifyEvent', '', true, false)]
    local procedure OnAfterModifySalesperson(var Rec: Record "Salesperson/Purchaser"; var xRec: Record "Salesperson/Purchaser")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Salesperson/Purchaser", 'OnAfterRenameEvent', '', true, false)]
    local procedure OnAfterRenameSalesperson(var Rec: Record "Salesperson/Purchaser"; var xRec: Record "Salesperson/Purchaser")
    begin
        ProcessRecordModifications(Rec, xRec, ActionEnumG::Update);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Salesperson/Purchaser", 'OnAfterDeleteEvent', '', true, false)]
    local procedure OnAfterDeleteSalesperson(var Rec: Record "Salesperson/Purchaser")
    begin
        ProcessRecordModifications(Rec, Rec, ActionEnumG::Delete);
    end;

    procedure ProcessRecordModifications(RecP: Variant; xRecP: Variant; ActionEnumP: Enum "CAR Actions")
    var
        DataTypeL: Codeunit "Data Type Management";
        RecRefL: RecordRef;
        FieldRefL: FieldRef;
        JsonObjectL: JsonObject;
    begin
        DataTypeL.GetRecordRef(RecP, RecRefL);
        if RecRefL.IsTemporary then
            exit;
        JsonObjectL := HandleRecord(ActionEnumP, RecP);
        FieldRefL := RecRefL.Field(RecRefL.SystemIdNo);
        SubmitJson(JsonObjectL, FieldRefL.Value);
    end;

    procedure SubmitJson(JsonObjectP: JsonObject; GuidP: Guid)
    var
        IntegrationSetupL: Record "CAR Integration Setup";
        Base64ConvL: Codeunit "Base64 Convert";
        HelperL: Codeunit "CAR Helper Library";
        RootObjectL: JsonObject;
        HeadersArrL: JsonArray;
        IdObjectL: JsonObject;
        KeyObjectL: JsonObject;
        ValueObjectL: JsonObject;
        TenantObjL: JsonObject;
        RoleObjL: JsonObject;
        UserObjL: JsonObject;
        HttpClientL: HttpClient;
        HttpContentL: HttpContent;
        HttpHeadersL: HttpHeaders;
        HttpResponseMessageL: HttpResponseMessage;
        PayloadL: Text;
        ValueL: Text;
    begin
        if not IntegrationSetupL.Get() then
            exit;

        TenantObjL.Add('name', 'TenantId');
        TenantObjL.Add('value', Base64ConvL.ToBase64(HelperL.GuidToJson(HelperL.Tenant())));
        RoleObjL.Add('name', 'RoleId');
        RoleObjL.Add('value', Base64ConvL.ToBase64(HelperL.GuidToJson(IntegrationSetupL."Role Id")));
        UserObjL.Add('name', 'UserId');
        UserObjL.Add('value', Base64ConvL.ToBase64(HelperL.GuidToJson(UserSecurityId())));

        HeadersArrL.Add(TenantObjL);
        HeadersArrL.Add(RoleObjL);
        HeadersArrL.Add(UserObjL);
        RootObjectL.Add('headers', HeadersArrL);

        IdObjectL.Add('id', HelperL.GuidToJson(GuidP));
        IdObjectL.WriteTo(ValueL);
        KeyObjectL.Add('type', 'BINARY');
        KeyObjectL.Add('data', Base64ConvL.ToBase64(ValueL));
        RootObjectL.Add('key', KeyObjectL);

        ValueObjectL.Add('type', 'BINARY');
        JsonObjectP.WriteTo(ValueL);
        ValueObjectL.Add('data', Base64ConvL.ToBase64(ValueL));
        RootObjectL.Add('value', ValueObjectL);

        RootObjectL.WriteTo(PayloadL);

        HttpContentL.WriteFrom(PayloadL);
        HttpContentL.GetHeaders(HttpHeadersL);

        HttpHeadersL.Clear();
        HttpHeadersL.Add('Content-Type', 'application/json');
        HttpClientL.DefaultRequestHeaders.Add('Accept', 'application/json');

        HttpClientL.Post(
            StrSubstNo('%1/%2/%3/%4/%5/%6/%7', IntegrationSetupL."Base URI", 'v3', 'clusters', IntegrationSetupL."Cluster ID",
                'topics', IntegrationSetupL."Setup Producer Topic", 'records'),
            HttpContentL,
            HttpResponseMessageL);
    end;

    procedure HandleRecord(ActionP: Enum "CAR Actions"; RecP: variant): JsonObject
    var
        RecRefL: RecordRef;
        JObjectL: JsonObject;
        JObject2L: JsonObject;
        JsonArrayL: JsonArray;
        ActionValueNameL: Text;
        RecordTypeJObject: JsonObject;
        EntityNameL: Text;
    begin
        RecRefL.GetTable(RecP);
        ActionValueNameL := ActionP.Names.Get(ActionP.Ordinals.IndexOf(ActionP.AsInteger()));
        JObjectL.Add('Action', ActionValueNameL);
        case RecRefL.Number of
            Database::Currency:
                begin
                    EntityNameL := 'Currency';
                    AddCurrencyPayload(RecRefL, JObject2L);
                end;
            Database::"VAT Product Posting Group":
                begin
                    EntityNameL := 'ProductTaxGroup';
                    AddVatProdPostingGroupPayload(RecRefL, JObject2L);
                end;
            Database::"VAT Business Posting Group":
                begin
                    EntityNameL := 'BusinessTaxGroup';
                    AddVatBussPostingGroupPayload(RecRefL, JObject2L);
                end;
            Database::"VAT Posting Setup":
                begin
                    EntityNameL := 'TaxSetup';
                    AddVatPostingSetupPayload(RecRefL, JObject2L);
                end;
            Database::"Company Information":
                begin
                    EntityNameL := 'CompanyInformation';
                    AddCompanyInformation(RecRefL, JObject2L);
                end;
            Database::Customer:
                begin
                    EntityNameL := 'Customer';
                    AddCustomerInformation(RecRefL, JObject2L);
                end;
            Database::"Salesperson/Purchaser":
                begin
                    EntityNameL := 'Salesperson';
                    AddSalesPersonInformation(RecRefL, JObject2L);
                end;

        end;
        JObjectL.Add('Entity', EntityNameL);
        JsonArrayL.Add(JObject2L);
        RecordTypeJObject.Add(EntityNameL, JsonArrayL);
        JObjectL.Add('Payload', RecordTypeJObject);

        exit(JObjectL);
    end;

    procedure AddSalesPersonInformation(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        SalespersonL: Record "Salesperson/Purchaser";
        TempBlobL: Codeunit "Temp Blob";
        Base64L: Codeunit "Base64 Convert";
        OutStreamL: OutStream;
        InStreamL: InStream;
        Base64TextL: Text;
    begin
        RecRefP.SetTable(SalespersonL);
        TempBlobL.CreateOutStream(OutStreamL);
        TempBlobL.CreateInStream(InStreamL);
        SalespersonL.Image.ExportStream(OutStreamL);
        Base64TextL := Base64L.ToBase64(InStreamL);

        JObjectP.Add('Id', GuidToJson(SalespersonL.SystemId));
        JObjectP.Add('Code', SalespersonL.Code);
        JObjectP.Add('DisplayName', SalespersonL.Name);
        JObjectP.Add('ModifiedOn', format(SalespersonL.SystemModifiedAt, 0, 9));
        JObjectP.Add('Email', SalespersonL."E-Mail");
        JObjectP.Add('Image', Base64TextL);
        JObjectP.Add('Phone', SalespersonL."Phone No.");
    end;

    procedure AddCustomerInformation(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        CustomerL: Record Customer;
        TempBlobL: Codeunit "Temp Blob";
        Base64L: Codeunit "Base64 Convert";
        AddressL: JsonObject;
        CommunicationL: JsonObject;
        OutStreamL: OutStream;
        InStreamL: InStream;
        Base64TextL: Text;
    begin
        RecRefP.SetTable(CustomerL);
        TempBlobL.CreateOutStream(OutStreamL);
        TempBlobL.CreateInStream(InStreamL);
        CustomerL.Image.ExportStream(OutStreamL);
        Base64TextL := Base64L.ToBase64(InStreamL);

        JObjectP.Add('Id', CreateGuid());//GuidToJson(CustomerL.SystemId));
        JObjectP.Add('Code', CustomerL."No.");
        JObjectP.Add('DisplayName', CustomerL.Name);
        JObjectP.Add('ModifiedOn', format(CustomerL.SystemModifiedAt, 0, 9));
        JObjectP.Add('Image', Base64TextL);
        JObjectP.Add('SalespersonCode', CustomerL."Salesperson Code");
        JObjectP.Add('Type', Format(CustomerL."Contact Type"));
        JObjectP.Add('VATRegistrationNo', CustomerL."VAT Registration No.");
        JObjectP.Add('Address', AddressL);
        JObjectP.Add('Communication', CommunicationL);
        AddressL.Add('Adress', CustomerL.Address);
        AddressL.Add('City', CustomerL.City);
        AddressL.Add('PostCode', CustomerL."Post Code");
        AddressL.Add('County', CustomerL.County);
        AddressL.Add('Country', CustomerL."Country/Region Code");
        CommunicationL.Add('Email', CustomerL."E-Mail");
        CommunicationL.Add('Phone', CustomerL."Phone No.");
        CommunicationL.Add('Mobile', CustomerL."Mobile Phone No.");
    end;

    procedure AddCurrencyPayload(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        CurrencyL: Record Currency;
    begin
        RecRefP.SetTable(CurrencyL);
        JObjectP.Add('Id', GuidToJson(CurrencyL.SystemId));
        JObjectP.Add('Code', CurrencyL.Code);
        JObjectP.Add('DisplayName', CurrencyL.Description);
        JObjectP.Add('AmountRndPrecision', CurrencyL."Amount Rounding Precision");
        JObjectP.Add('AmountRndType', 'Nearest');
        JObjectP.Add('UnitAmountRndPrecision', CurrencyL."Unit-Amount Rounding Precision");
        JObjectP.Add('UnitAmountRndType', 'Nearest');
        JObjectP.Add('VatRndPrecision', CurrencyL."Amount Rounding Precision");
        JObjectP.Add('VatRndType', Format(CurrencyL."VAT Rounding Type", 0, 0));
        JObjectP.Add('Symbol', CurrencyL.Symbol);
    end;

    procedure AddCompanyInformation(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        CompInfoL: Record "Company Information";
    begin
        RecRefP.SetTable(CompInfoL);
        JObjectP.Add('Id', GuidToJson(CompInfoL.SystemId));
        JObjectP.Add('CompanyName', CompInfoL.Name);
        JObjectP.Add('Address', CompInfoL.Address);
        JObjectP.Add('ZipCode', CompInfoL."Post Code");
        JObjectP.Add('CountryId', CompInfoL."Country/Region Code");
        JObjectP.Add('WebSite', CompInfoL."Home Page");
        JObjectP.Add('TaxNumber', CompInfoL."VAT Registration No.");
        JObjectP.Add('PhoneNo', CompInfoL."Phone No.");
        JObjectP.Add('Email', CompInfoL."E-Mail");
    end;

    procedure AddVatPostingSetupPayload(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        VatPostingSetupL: Record "VAT Posting Setup";
    begin
        RecRefP.SetTable(VatPostingSetupL);
        JObjectP.Add('Id', GuidToJson(VatPostingSetupL.SystemId));
        JObjectP.Add('BusinessTaxGroupId',
            GuidToJson(GetVatBussPostingGroupSystemId(VatPostingSetupL."VAT Bus. Posting Group")));
        JObjectP.Add('ProductTaxGroupId',
            GuidToJson(GetVatProdPostingGroupSystemId(VatPostingSetupL."VAT Prod. Posting Group")));
        JObjectP.Add('TaxCode', VatPostingSetupL."VAT Identifier");
        JObjectP.Add('TaxType', Format(VatPostingSetupL."VAT Calculation Type", 0, 0));
        JObjectP.Add('VAT%', VatPostingSetupL."VAT %");
        JObjectP.Add('ModifiedOn', format(VatPostingSetupL.SystemModifiedAt, 0, 9));
    end;

    procedure AddVatProdPostingGroupPayload(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        VatProdPostingGroupL: Record "VAT Product Posting Group";
    begin
        RecRefP.SetTable(VatProdPostingGroupL);
        JObjectP.Add('Id', GuidToJson(VatProdPostingGroupL.SystemId));
        JObjectP.Add('Code', VatProdPostingGroupL.Code);
        JObjectP.Add('DisplayName', VatProdPostingGroupL.Description);
        JObjectP.Add('ModifiedOn', format(VatProdPostingGroupL.SystemModifiedAt, 0, 9));
    end;

    procedure AddVatBussPostingGroupPayload(var RecRefP: RecordRef; var JObjectP: JsonObject)
    var
        VatBussPostingGroupL: Record "VAT Business Posting Group";
    begin
        RecRefP.SetTable(VatBussPostingGroupL);
        JObjectP.Add('Id', GuidToJson(VatBussPostingGroupL.SystemId));
        JObjectP.Add('Code', VatBussPostingGroupL.Code);
        JObjectP.Add('DisplayName', VatBussPostingGroupL.Description);
        JObjectP.Add('ModifiedOn', format(VatBussPostingGroupL.SystemModifiedAt, 0, 9));
    end;

    procedure GetVatProdPostingGroupSystemId(CodeP: Code[20]): Guid
    var
        VatProdPostingGrpL: Record "VAT Product Posting Group";
    begin
        if VatProdPostingGrpL.GET(CodeP) then
            exit(VatProdPostingGrpL.SystemId);
    end;

    procedure GetVatBussPostingGroupSystemId(CodeP: Code[20]): Guid
    var
        BussPostingGroupL: Record "VAT Business Posting Group";
    begin
        if BussPostingGroupL.GET(CodeP) then
            exit(BussPostingGroupL.SystemId);
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

    var
        ActionEnumG: Enum "CAR Actions";
}