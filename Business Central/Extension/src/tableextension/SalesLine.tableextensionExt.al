tableextension 50102 "CAR Sales Line" extends "Sales Line"
{
    fields
    {
        modify("No.")
        {
            TableRelation = IF (Type = CONST(" ")) "Standard Text"
            ELSE
            IF (Type = CONST("G/L Account"), "System-Created Entry" = CONST(false)) "G/L Account" WHERE("Direct Posting" = CONST(true),
                                                                                               "Account Type" = CONST(Posting),
                                                                                               Blocked = CONST(false))
            ELSE
            IF (Type = CONST("G/L Account"), "System-Created Entry" = CONST(true)) "G/L Account"
            ELSE
            IF (Type = CONST(Resource)) Resource
            ELSE
            IF (Type = CONST("Fixed Asset")) "Fixed Asset"
            ELSE
            IF (Type = CONST("Charge (Item)")) "Item Charge"
            ELSE
            IF (Type = CONST(Item), "Document Type" = FILTER(<> "Credit Memo" & <> "Return Order")) Item WHERE(Blocked = CONST(false),
                                                                                                                                                                                       "Sales Blocked" = CONST(false))
            ELSE
            IF (Type = CONST(Item), "Document Type" = FILTER("Credit Memo" | "Return Order")) Item WHERE(Blocked = CONST(false))
            else
            if (Type = const(Vehicle)) Item WHERE(Blocked = const(false), Type = const(Model))
            else
            if (Type = const("Vehicle Option")) Item WHERE(Blocked = const(false), Type = const(Option));
        }

        field(50100; "CAR Brand"; Code[20])
        {
            Caption = 'Brand';
            DataClassification = ToBeClassified;
        }
    }
}
