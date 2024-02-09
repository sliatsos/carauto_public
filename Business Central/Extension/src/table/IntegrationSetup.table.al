table 50100 "CAR Integration Setup"
{
    Caption = 'CAR Integration Setup';
    DataClassification = CustomerContent;

    fields
    {
        field(1; "Primary Key"; Code[20])
        {
            Caption = 'Primary Key';
            DataClassification = CustomerContent;
        }
        field(21; "Consumer Instance Id"; Guid)
        {
            Caption = 'Consumer Instance Id';
            DataClassification = CustomerContent;
        }
        field(22; "Consumer Id"; Guid)
        {
            Caption = 'Consumer Id';
            DataClassification = CustomerContent;
        }
        field(23; "Base URI"; Text[1024])
        {
            Caption = 'Base URI';
            DataClassification = CustomerContent;

            trigger OnValidate()
            begin
                if Rec."Base URI".EndsWith('/') then
                    Rec."Base URI" := Rec."Base URI".TrimEnd('/');
            end;
        }
        field(24; "Consumer Topic"; Text[250])
        {
            Caption = 'Consumer Topic';
            DataClassification = CustomerContent;
        }
        field(25; Enabled; Boolean)
        {
            Caption = 'Enabled';
            DataClassification = CustomerContent;
        }
        field(26; "Cluster Id"; Text[250])
        {
            Caption = 'Cluster Id';
            DataClassification = CustomerContent;
        }
        field(27; "Setup Producer Topic"; Text[250])
        {
            Caption = 'Setup Producer Topic';
            DataClassification = CustomerContent;
        }
        field(28; "Role Id"; Guid)
        {
            Caption = 'Role Id';
            DataClassification = CustomerContent;
        }
        field(29; "Item Template Code"; Code[20])
        {
            Caption = 'Item Template Code';
            TableRelation = "Item Templ.";
        }
        field(30; "Order Consumer Instance Id"; Guid)
        {
            Caption = 'Order Consumer Instance Id';
            DataClassification = CustomerContent;
        }
        field(31; "Order Consumer Id"; Guid)
        {
            Caption = 'Order Consumer Id';
            DataClassification = CustomerContent;
        }
        field(32; "Order Consumer Topic"; Text[250])
        {
            Caption = 'Order Consumer Topic';
            DataClassification = CustomerContent;
        }
        field(33; "Order Producer Topic"; Text[250])
        {
            Caption = 'Order Producer Topic';
            DataClassification = CustomerContent;
        }
    }
    keys
    {
        key(PK; "Primary Key")
        {
            Clustered = true;
        }
    }
}
