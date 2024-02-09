tableextension 50100 "CAR Sales Header" extends "Sales Header"
{
    fields
    {
        field(50100; "CAR Kafka Status"; Enum "CAR Kafka Status")
        {
            Caption = 'Kafka Status';
            DataClassification = CustomerContent;
        }
        field(50101; "CAR Tenant Id"; Guid)
        {
            Caption = 'Tenant Id';
            DataClassification = CustomerContent;
        }
        field(50102; "CAR Posted Document Id"; Guid)
        {
            Caption = 'Posted Document Id';
            DataClassification = CustomerContent;
        }
        field(50103; "CAR Document Type"; Enum "CAR Document Type")
        {
            Caption = 'Document Type';
            DataClassification = CustomerContent;
        }
        field(50104; "CAR VIN"; Code[20])
        {
            Caption = 'VIN';
            DataClassification = CustomerContent;
        }
        field(50105; "CAR License No."; Code[20])
        {
            Caption = 'License Number';
            DataClassification = CustomerContent;
        }
        field(50106; "CAR Registration Date"; Date)
        {
            Caption = 'Registration Date';
            DataClassification = CustomerContent;
        }
        field(50107; "CAR Model Code"; Code[20])
        {
            Caption = 'Model Code';
            DataClassification = CustomerContent;
        }
        field(50108; "CAR Model Description"; Text[100])
        {
            Caption = 'Model Description';
            DataClassification = CustomerContent;
        }
        field(50109; "CAR Model Year"; Integer)
        {
            Caption = 'Model Year';
            DataClassification = CustomerContent;
        }
        field(50110; "CAR Brand"; Code[20])
        {
            Caption = 'Brand';
            DataClassification = CustomerContent;
        }
        field(50111; "CAR Job Queue Entry ID"; Guid)
        {
        }
        field(50112; "CAR Is Vehicle Order"; Boolean)
        {
            Caption = 'Is Vehicle Order';
            DataClassification = CustomerContent;
        }
        field(50114; "CAR Quote Id"; Guid)
        {
            Caption = 'Quote Id';
            DataClassification = CustomerContent;
        }
    }
}
