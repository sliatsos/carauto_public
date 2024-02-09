table 50102 "CAR Brand"
{
    Caption = 'Brand';
    DataClassification = ToBeClassified;

    fields
    {
        field(1; "Code"; Code[20])
        {
            Caption = 'Code';
        }
        field(21; Description; Text[200])
        {
            Caption = 'Description';
        }
        field(22; Image; Media)
        {
            Caption = 'Image';
            ExtendedDatatype = Person;
        }
    }
    keys
    {
        key(PK; "Code")
        {
            Clustered = true;
        }
    }
}
