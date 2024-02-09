pageextension 50100 "CAR Item List" extends "Item List"
{
    layout
    {
        addlast(content)
        {
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
