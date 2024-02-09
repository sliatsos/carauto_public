codeunit 50103 "CAR Kafka Sender"
{
    var
        HelperG: Codeunit "CAR Helper Library";

    trigger OnRun()
    begin
        SendInvoiceDocumentsToKafka();
        SendOrderDocumentsToKafka();
    end;

    local procedure SendOrderDocumentsToKafka()
    var
        SalesHeaderL: Record "Sales Header";
        JOrderL: JsonObject;
    begin
        // Send the posted documents
        SalesHeaderL.Reset();
        SalesHeaderL.SetRange("CAR Is Vehicle Order", true);
        SalesHeaderL.SetRange("CAR Kafka Status", "CAR Kafka Status"::OrderCreated);
        if SalesHeaderL.FindSet() then
            repeat
                JOrderL := HelperG.CreateSalesDocumentJson(SalesHeaderL);
                if WrapObjectAndSend(JOrderL, 'Quote To Order', SalesHeaderL.SystemId, SalesHeaderL."Car Tenant Id", 'Header') then begin
                    SalesHeaderL."CAR Kafka Status" := "CAR Kafka Status"::ToBePosted;
                    SalesHeaderL.Modify();
                end;
            until SalesHeaderL.Next() = 0;

        // Send documents with error
        SalesHeaderL.SetRange("CAR Kafka Status", "CAR Kafka Status"::Error);
        if SalesHeaderL.FindSet() then
            repeat
                if SendErrorToKafka('001',
                    GetErrorFromJobQueueEntry(SalesHeaderL),
                    HelperG.GuidToJson(SalesHeaderL."CAR Posted Document Id"),
                    'Backend Quote to Order Failure',
                    SalesHeaderL."CAR Tenant Id")
                then begin
                    SalesHeaderL."CAR Kafka Status" := "CAR Kafka Status"::Sent;
                    SalesHeaderL.Modify();
                end;
            until SalesHeaderL.Next() = 0;
    end;

    local procedure SendInvoiceDocumentsToKafka()
    var
        SalesInvHdrL: Record "Sales Invoice Header";
        JOrderL: JsonObject;
    begin
        // Send the posted documents
        SalesInvHdrL.Reset();
        SalesInvHdrL.SetRange("CAR Kafka Status", SalesInvHdrL."CAR Kafka Status"::Posted);
        if SalesInvHdrL.FindSet() then
            repeat
                JOrderL := HelperG.CreateSalesDocumentJson(SalesInvHdrL);
                if WrapObjectAndSend(JOrderL, 'Backend Invoiced', SalesInvHdrL.SystemId, SalesInvHdrL."Car Tenant Id", 'Header') then begin
                    SalesInvHdrL."CAR Kafka Status" := "CAR Kafka Status"::Sent;
                    SalesInvHdrL.Modify();
                end;
            until SalesInvHdrL.Next() = 0;

        // Send documents with error
        SalesInvHdrL.SetRange("CAR Kafka Status", "CAR Kafka Status"::Error);
        if SalesInvHdrL.FindSet() then
            repeat
                if SendErrorToKafka('001',
                    GetErrorFromJobQueueEntry(SalesInvHdrL),
                    HelperG.GuidToJson(SalesInvHdrL."CAR Posted Document Id"),
                    'Backend Invoicing Failure',
                    SalesInvHdrL."CAR Tenant Id")
                then begin
                    SalesInvHdrL."CAR Kafka Status" := "CAR Kafka Status"::Sent;
                    SalesInvHdrL.Modify();
                end;
            until SalesInvHdrL.Next() = 0;
    end;

    local procedure GetErrorFromJobQueueEntry(SalesInvHdrP: Record "Sales Invoice Header"): Text
    var
        JobQueueLogL: Record "Job Queue Log Entry";
    begin
        JobQueueLogL.Reset();
        JobQueueLogL.SetRange(ID, SalesInvHdrP."CAR Job Queue Entry ID");
        JobQueueLogL.SetRange(Status, JobQueueLogL.Status::Error);
        if JobQueueLogL.FindLast() then
            exit(JobQueueLogL."Error Message");
    end;

    local procedure GetErrorFromJobQueueEntry(SalesHdrP: Record "Sales Header"): Text
    var
        JobQueueLogL: Record "Job Queue Log Entry";
    begin
        JobQueueLogL.Reset();
        JobQueueLogL.SetRange(ID, SalesHdrP."CAR Job Queue Entry ID");
        JobQueueLogL.SetRange(Status, JobQueueLogL.Status::Error);
        if JobQueueLogL.FindLast() then
            exit(JobQueueLogL."Error Message");
    end;

    [TryFunction]
    local procedure SendErrorToKafka(CodeP: Text; MessageP: Text; KeyP: Text; StateP: Text; TenantIdP: Guid)
    var
        SalesIntegrSetupL: Record "CAR Integration Setup";
        JObjectL: JsonObject;
    begin
        if not SalesIntegrSetupL.Get() then
            exit;
        JObjectL.Add('Code', CodeP);
        JObjectL.Add('Message', MessageP);
        WrapObjectAndSend(JObjectL, StateP, KeyP, TenantIdP, 'Error');
    end;

    [TryFunction]
    local procedure WrapObjectAndSend(JObjectP: JsonObject; StateP: Text; KeyP: Guid; TenantIdP: Guid; ObjNameP: Text)
    var
        JWrapL: JsonObject;
    begin
        JWrapL.Add('State', StateP);
        JWrapL.Add(ObjNameP, JObjectP);
        SendInvoiceCreditResponse(JWrapL, HelperG.GuidToJson(KeyP), TenantIdP);
    end;

    local procedure SendInvoiceCreditResponse(var JObjectP: JsonObject; SystemIdP: Guid; TenantIdP: Guid)
    var
        IntegrationSetupL: Record "CAR Integration Setup";
        Base64ConvL: Codeunit "Base64 Convert";
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
        TenantObjL.Add('value', Base64ConvL.ToBase64(HelperG.GuidToJson(TenantIdP)));
        RoleObjL.Add('name', 'RoleId');
        RoleObjL.Add('value', Base64ConvL.ToBase64(HelperG.GuidToJson(TenantIdP)));
        UserObjL.Add('name', 'UserId');
        UserObjL.Add('value', Base64ConvL.ToBase64(HelperG.GuidToJson(TenantIdP)));

        HeadersArrL.Add(TenantObjL);
        HeadersArrL.Add(RoleObjL);
        HeadersArrL.Add(UserObjL);
        RootObjectL.Add('headers', HeadersArrL);

        IdObjectL.Add('id', HelperG.GuidToJson(SystemIdP));
        IdObjectL.WriteTo(ValueL);
        KeyObjectL.Add('type', 'BINARY');
        KeyObjectL.Add('data', Base64ConvL.ToBase64(ValueL));
        RootObjectL.Add('key', KeyObjectL);

        ValueObjectL.Add('type', 'BINARY');
        JObjectP.WriteTo(ValueL);
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
                'topics', IntegrationSetupL."Order Producer Topic", 'records'),
            HttpContentL,
            HttpResponseMessageL);
    end;
}
