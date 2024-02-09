page 50102 "CAR Brands"
{
    ApplicationArea = All;
    Caption = 'Brands';
    PageType = List;
    SourceTable = "CAR Brand";
    UsageCategory = Administration;

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(Image; Rec.Image)
                {
                }
                field("Code"; Rec."Code")
                {
                }
                field(Description; Rec.Description)
                {
                }
            }
        }
    }
}
