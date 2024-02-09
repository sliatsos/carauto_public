page 50101 "CAR Tenants"
{
    ApplicationArea = All;
    Caption = 'Tenants';
    PageType = List;
    SourceTable = "CAR Tenant";

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(Id; Rec.Id)
                {
                    Visible = false;
                    ToolTip = 'Specifies the value of the Id field.';
                }
                field(Tenant; Rec.Tenant)
                {
                    ToolTip = 'Specifies the value of the Tenant field.';
                }
            }
        }
    }
}
