table 50101 "CAR Tenant"
{
    Caption = 'Tenant';
    DataClassification = ToBeClassified;

    fields
    {
        field(1; Id; Guid)
        {
            Caption = 'Id';
            DataClassification = ToBeClassified;
        }
        field(21; Tenant; Text[250])
        {
            Caption = 'Tenant';
            DataClassification = ToBeClassified;
        }
    }
    keys
    {
        key(PK; Id)
        {
            Clustered = true;
        }
    }
}
