codeunit 50102 "CAR Service Connection Handler"
{
    [EventSubscriber(ObjectType::Table, Database::"Service Connection", 'OnRegisterServiceConnection', '', true, true)]
    local procedure HandleEDIVendorSetupRegisterServiceConnection(var ServiceConnection: Record "Service Connection" temporary)
    var
        IntegrationSetupL: Record "CAR Integration Setup";
        IntegrationSetupCardL: Page "CAR Integration Setup";
        RecordRefL: RecordRef;
        PageIdL: Integer;
        PageCaptionL: Text;
    begin
        RecordRefL.GetTable(IntegrationSetupL);
        if not (RecordRefL.ReadPermission() and RecordRefL.WritePermission()) then
            exit;
        PageIdL := page::"CAR Integration Setup";
        PageCaptionL := IntegrationSetupCardL.Caption();

        IntegrationSetupL.SetRange(Enabled, true);
        if not IntegrationSetupL.IsEmpty then
            ServiceConnection.Status := ServiceConnection.Status::Enabled
        else
            ServiceConnection.Status := ServiceConnection.Status::Disabled;

        ServiceConnection.InsertServiceConnection(
            ServiceConnection, IntegrationSetupL.RecordId, PageCaptionL, '', PageIdL);
    end;
}
