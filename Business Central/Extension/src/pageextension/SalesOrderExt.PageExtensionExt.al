pageextension 50103 "CAR Sales Order Ext." extends "Sales Order"
{
    layout
    {
        addafter(General)
        {
            group("CAR Vehicle Information")
            {
                Caption = 'Vehicle Information';

                field("CAR Brand"; Rec."CAR Brand")
                {
                    ToolTip = 'Brand Code of the Model';
                    Editable = false;
                    ApplicationArea = All;
                }
                field("CAR Model Code"; Rec."CAR Model Code")
                {
                    ToolTip = 'Model Code';
                    Editable = false;
                    ApplicationArea = All;
                }
                field("CAR Model Year"; Rec."CAR Model Year")
                {
                    ToolTip = 'Model Year';
                    Editable = false;
                    ApplicationArea = All;
                }
                field("CAR Model Description"; Rec."CAR Model Description")
                {
                    ToolTip = 'Model Description';
                    Editable = false;
                    ApplicationArea = All;
                }
                field("CAR VIN"; Rec."CAR VIN")
                {
                    ToolTip = 'VIN';
                    ApplicationArea = All;
                }
                field("CAR License No."; Rec."CAR License No.")
                {
                    ToolTip = 'License Number';
                    ApplicationArea = All;
                }
                field("CAR Registration Number"; Rec."CAR Registration Date")
                {
                    ToolTip = 'Registration Date';
                    Editable = false;
                    ApplicationArea = All;
                }
            }
        }
    }
}
