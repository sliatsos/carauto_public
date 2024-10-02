page 50100 "CAR Integration Setup"
{
    ApplicationArea = All;
    Caption = 'Integration Setup';
    PageType = Card;
    SourceTable = "CAR Integration Setup";
    AboutTitle = 'About Integration Setup';
    AboutText = 'In this setup you can set the connection parameters to the kafka cluster with which your system will interact with';

    layout
    {
        area(content)
        {
            group(General)
            {
                Caption = 'General';

                field(Enabled; Rec.Enabled)
                {
                    AboutTitle = 'About Enabled';
                    AboutText = 'The Enabled control will enable the integration functionality. When disabled the integration will stop.';

                    trigger OnValidate()
                    var
                        HelperL: Codeunit "CAR Helper Library";
                    begin
                        HelperL.GetClusterId(Rec);
                    end;
                }
                field("Base URI"; Rec."Base URI")
                {
                    AboutTitle = 'About Base URI';
                    AboutText = 'The Base Uri is the location of the kafka cluster.';
                }
                group(GeneralAdvanced)
                {
                    Visible = IsAdvancedG;
                    ShowCaption = false;
                    field("Cluster Id"; Rec."Cluster Id")
                    {
                        AboutTitle = 'About Cluster Id';
                        AboutText = 'The Cluster Id keeps the Kafka Cluster identification number which is needed for the communication. It will be automatically populated when the Base Uri is populated and the Enabled is set to true.';
                    }
                }
            }
            group("Data Synchronization")
            {
                Caption = 'Data Synchronization';
                AboutTitle = 'About Data Synchronization';
                AboutText = 'The fields in this group are used to setup the consumer for the data synchronization from the external system to the ERP';
                field("Consumer Topic"; Rec."Consumer Topic")
                {
                    AboutTitle = 'About Consumer Topic';
                    AboutText = 'The topic in which we will listen for changes that we have to integrate into the system. It must be manually set.';
                }
                field("Setup Producer Topic"; Rec."Setup Producer Topic")
                {
                    AboutTitle = 'About Setup Producer Topic';
                    AboutText = 'It is used to publish messages to kafka when a record has been modified in business central.';
                }
                group("Data Synchronization Advanced")
                {
                    Visible = IsAdvancedG;
                    ShowCaption = false;
                    field("Consumer Instance Id"; Rec."Consumer Instance Id")
                    {
                        AboutTitle = 'About Consumer Instance Id';
                        AboutText = 'Internal Functionality';
                    }
                    field("Consumer Id"; Rec."Consumer Id")
                    {
                        AboutTitle = 'About Consumer Id';
                        AboutText = 'Internal Functionality';
                    }
                }
            }
            group(Ordering)
            {
                Caption = 'Ordering';
                AboutTitle = 'About Ordering';
                AboutText = 'The fields in this group are used to setup the consumer for the data synchronization for the orders from the external system to the ERP';

                field("Order Consumer Topic"; Rec."Order Consumer Topic")
                {
                    Caption = 'Consumer Topic';
                    AboutTitle = 'About Consumer Topic';
                    AboutText = 'The topic in which we will listen for changes that we have to integrate into the system. It must be manually set.';
                }

                field("Order Producer Topic"; Rec."Order Producer Topic")
                {
                    Caption = 'Producer Topic';
                    AboutTitle = 'About Producer Topic';
                    AboutText = 'The topic in which we will push changes that we have to integrate into the system. It must be manually set.';
                }
                group("Ordering Advanced")
                {
                    Visible = IsAdvancedG;
                    ShowCaption = false;

                    field("Order Consumer Instance Id"; Rec."Order Consumer Instance Id")
                    {
                        Caption = 'Consumer Instance Id';
                        AboutTitle = 'About Consumer Instance Id';
                        AboutText = 'Internal Functionality';
                    }
                    field("Order Consumer Id"; Rec."Order Consumer Id")
                    {
                        Caption = 'Consumer Id';
                        AboutTitle = 'About Consumer Id';
                        AboutText = 'Internal Functionality';
                    }
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action("Init Setup")
            {
                Caption = 'Initialize Setup';
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                Enabled = Rec.Enabled;

                trigger OnAction()
                var
                    SetupUpdaterL: Codeunit "CAR Setup Sync. Subscriber";
                    RecRefL: RecordRef;
                    TableListL: List of [Integer];
                    TableIdL: Integer;
                begin
                    TableListL.Add(Database::"VAT Business Posting Group");
                    // TableListL.Add(Database::"VAT Product Posting Group");
                    // TableListL.Add(Database::"VAT Posting Setup");
                    // TableListL.Add(Database::"Company Information");
                    // TableListL.Add(Database::Currency);
                    TableListL.Add(Database::"Salesperson/Purchaser");
                    TableListL.Add(Database::Customer);

                    foreach TableIdL in TableListL do begin
                        RecRefL.Open(TableIdL);
                        if RecRefL.FindSet() then
                            repeat
                                SetupUpdaterL.ProcessRecordModifications(RecRefL, RecRefL, "CAR Actions"::Insert);
                            until RecRefL.Next() = 0;
                        RecRefL.Close();
                    end;
                end;
            }

            action(Advanced)
            {
                Caption = 'Advanced';
                Promoted = true;
                PromotedIsBig = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                AboutTitle = 'Advanced';
                AboutText = 'It will hide or show the advanced settings of the page';

                trigger OnAction()
                begin
                    IsAdvancedG := not IsAdvancedG;
                end;
            }
        }
    }

    var
        IsAdvancedG: Boolean;

    trigger OnOpenPage()
    begin
        if not Rec.Get() then
            Rec.Insert();
    end;
}
