tableextension 50103 "CAR Item" extends Item
{
    fields
    {
        field(50100; "CAR Internal Code"; Code[20])
        {
            Caption = 'CAR Internal Code';
            DataClassification = ToBeClassified;
        }
        field(50101; "CAR Type"; Enum "CAR Option Type")
        {
            Caption = 'Option Type';
            DataClassification = ToBeClassified;
        }
        field(50102; "CAR Brand Code"; Code[20])
        {
            Caption = 'Brand';
            TableRelation = "CAR Brand";
        }
        field(50103; "CAR Model Year"; Integer)
        {
            Caption = 'Model Year';
        }
    }
}
