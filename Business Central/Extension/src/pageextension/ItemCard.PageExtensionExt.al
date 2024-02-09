pageextension 50101 "CAR Item Card" extends "Item Card"
{
    layout
    {
        addlast(content)
        {
            group("CAR Group")
            {

                Caption = 'CAR';

                field("CAR Type"; Rec."CAR Type")
                {
                    ApplicationArea = All;
                }
                field("CAR Internal Code"; Rec."CAR Internal Code")
                {
                    ApplicationArea = All;
                }
                field("CAR Brand Code"; Rec."CAR Brand Code")
                {
                    ApplicationArea = All;
                }
                field("CAR Model Year"; Rec."CAR Model Year")
                {
                    ApplicationArea = All;
                }
            }
        }
    }
}
