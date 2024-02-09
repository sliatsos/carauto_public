codeunit 50108 "CAR Order Kafka Receiver"
{
    var
        HelperG: Codeunit "CAR Helper Library";


    trigger OnRun()
    begin
        ReceiveRecords();
    end;

    local procedure ReceiveRecords()
    var
        TempBlobL: Codeunit "Temp Blob";
        JObjectL: JsonObject;
        JDocL: JsonObject;
        JArrayL: JsonArray;
        JTokenL: JsonToken;
        JToken2L: JsonToken;
        OutStreamL: OutStream;
    begin
        if not GetRecordsFromKafka(JObjectL, JArrayL) then begin
            InitiateConnectionToKafka();
            if GetRecordsFromKafka(JObjectL, JArrayL) then
                ;
        end;
        foreach JTokenL in JArrayL do begin
            JTokenL.AsObject().Get('value', JToken2L);
            JDocL := JToken2L.AsObject();

            TempBlobL.CreateOutStream(OutStreamL);
            JDocL.WriteTo(OutStreamL);

            OnKafkaMessageReceivedEvent(TempBlobL);
            Commit();
        end;
    end;

    local procedure InitiateConnectionToKafka()
    var
        ConsumerInstanceUrlL: Text;
    begin
        ConsumerInstanceUrlL := CreateConsumerInstance();
        CreateSubscriberFromConsumer(ConsumerInstanceUrlL);
    end;

    local procedure CreateSubscriberFromConsumer(ConsumerInstanceUrlP: Text)
    var
        SalesIntegrSetupL: Record "CAR Integration Setup";
        JSonObjL: JsonObject;
        JArrayL: JsonArray;
    begin
        SalesIntegrSetupL.Get();
        JArrayL.Add(SalesIntegrSetupL."Order Consumer Topic");
        JSonObjL.Add('topics', JArrayL);
        SubmitJson(JSonObjL, StrSubstNo('%1/%2', ConsumerInstanceUrlP, 'subscription'));
    end;

    local procedure CreateConsumerInstance(): Text
    var
        IntegrationSetupL: Record "CAR Integration Setup";
        JObjectL: JsonObject;
        UrlL: Text;
        ResponseL: Text;
        JsonTokenL: JsonToken;
    begin
        IntegrationSetupL.Get();
        if IsNullGuid(IntegrationSetupL."Order Consumer Instance Id") then begin
            IntegrationSetupL."Order Consumer Instance Id" := HelperG.GuidToJson(CreateGuid());
            IntegrationSetupL."Order Consumer Id" := HelperG.GuidToJson(CreateGuid());
            IntegrationSetupL.Modify();
        end;
        JObjectL.Add('name', IntegrationSetupL."Order Consumer Id");
        JObjectL.Add('format', 'json');
        JObjectL.Add('auto.offset.reset', 'earliest');
        UrlL := StrSubstNo('%1/%2/%3', IntegrationSetupL."Base URI", 'consumers', IntegrationSetupL."Order Consumer Instance Id");
        ResponseL := SubmitJson(JObjectL, UrlL);
        JObjectL.ReadFrom(ResponseL);
        JObjectL.Get('instance_id', JsonTokenL);
        // SalesIntegrSetupL."Consumer Instance Url" := StrSubstNo('%1/%2/%3', UrlL, 'instances', JsonTokenL.AsValue().AsText());
        IntegrationSetupL.Modify();
        // exit(SalesIntegrSetupL."Consumer Instance Url");
        exit(StrSubstNo('%1/%2/%3', UrlL, 'instances', JsonTokenL.AsValue().AsText()));
    end;

    local procedure SubmitJson(JsonObjectP: JsonObject; UrlP: Text) ResponseR: Text
    var
        HttpClientL: HttpClient;
        HttpContentL: HttpContent;
        HttpHeadersL: HttpHeaders;
        HttpResponseMessageL: HttpResponseMessage;
        PayloadL: Text;
    begin
        JsonObjectP.WriteTo(PayloadL);
        HttpContentL.WriteFrom(PayloadL);
        HttpContentL.GetHeaders(HttpHeadersL);

        HttpHeadersL.Clear();
        HttpHeadersL.Add('Content-Type', 'application/vnd.kafka.v2+json');

        HttpClientL.Post(UrlP, HttpContentL, HttpResponseMessageL);
        if not HttpResponseMessageL.IsSuccessStatusCode then
            Error('%1-%2', HttpResponseMessageL.HttpStatusCode, HttpResponseMessageL.ReasonPhrase);
        HttpResponseMessageL.Content.ReadAs(ResponseR);
    end;

    [TryFunction]
    local procedure GetRecordsFromKafka(var JObjectP: JsonObject; var JArrayP: JsonArray)
    var
        IntegrationSetupL: Record "CAR Integration Setup";
        ResponseL: Text;
        JTokenL: JsonToken;
        JArrayL: JsonArray;
        JKeyL: JsonObject;
        JToke2L: JsonToken;
        JObjectL: JsonObject;
        KeyValueObjL: JsonObject;
        UrlL: Text;
    begin
        IntegrationSetupL.Get();
        IntegrationSetupL.TestField("Order Consumer Instance Id");
        IntegrationSetupL.TestField("Order Consumer Id");
        UrlL := StrSubstNo('%1/%2/%3', IntegrationSetupL."Base URI", 'consumers', IntegrationSetupL."Order Consumer Instance Id");
        UrlL := StrSubstNo('%1/%2/%3', UrlL, 'instances', IntegrationSetupL."Order Consumer Id");

        ResponseL := SubmitRequest(StrSubstNo('%1/%2', UrlL, 'records'));
        if TryReadAsJObject(JObjectP, ResponseL) then begin
            if JObjectP.Get('error_code', JTokenL) then
                Error('');
        end else begin
            JArrayL.ReadFrom(ResponseL);
            foreach JToke2L in JArrayL do begin
                Clear(JObjectL);
                Clear(KeyValueObjL);
                JToke2L.AsObject().Get('value', JTokenL);
                JObjectL := JTokenL.AsObject();
                JToke2L.AsObject().Get('key', JTokenL);
                JKeyL := JTokenL.AsObject();
                JKeyL.Get('id', JTokenL);
                if JObjectL.Add('KafkaKey', JTokenL.AsValue().AsText()) then
                    ;
                KeyValueObjL.Add('value', JObjectL);

                JArrayP.Add(KeyValueObjL);
            end;
        end;
    end;

    [TryFunction]
    local procedure TryReadAsJObject(var JObjectP: JsonObject; InputP: Text)
    begin
        JObjectP.ReadFrom(InputP);
    end;

    local procedure SubmitRequest(UrlP: Text) ResponseR: Text
    var
        HttpClientL: HttpClient;
        HttpResponseMessageL: HttpResponseMessage;
    begin
        HttpClientL.DefaultRequestHeaders.Add('Accept', 'application/vnd.kafka.json.v2+json');
        if not HttpClientL.Get(UrlP, HttpResponseMessageL) then
            Error('%1-%2', HttpResponseMessageL.HttpStatusCode, HttpResponseMessageL.ReasonPhrase);
        HttpResponseMessageL.Content.ReadAs(ResponseR);
    end;

    [BusinessEvent(false, true)]
    local procedure OnKafkaMessageReceivedEvent(var TempBlobP: Codeunit "Temp Blob")
    begin
    end;
}
