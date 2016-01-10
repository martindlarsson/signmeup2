namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DynamiskaFaltv1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Banor", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Registreringar", "Bana_Id", "dbo.Banor");
            DropForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar");
            DropForeignKey("dbo.Registreringar", "Kanot_Id", "dbo.Kanoter");
            DropForeignKey("dbo.Registreringar", "Klass_Id", "dbo.Klasser");
            DropForeignKey("dbo.Kanoter", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Klasser", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Registreringar", "EvenemangsId", "dbo.Evenemang");
            DropIndex("dbo.Banor", new[] { "EvenemangsId" });
            DropIndex("dbo.Kanoter", new[] { "EvenemangsId" });
            DropIndex("dbo.Registreringar", new[] { "EvenemangsId" });
            DropIndex("dbo.Registreringar", new[] { "Bana_Id" });
            DropIndex("dbo.Registreringar", new[] { "Kanot_Id" });
            DropIndex("dbo.Registreringar", new[] { "Klass_Id" });
            DropIndex("dbo.Deltagare", new[] { "RegistreringarID" });
            DropIndex("dbo.Klasser", new[] { "EvenemangsId" });
            CreateTable(
                "dbo.Formular",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvenemangsId = c.Int(nullable: false),
                        Namn = c.String(nullable: false),
                        Gratis = c.Boolean(nullable: false),
                        Startavgift = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.WizardSteps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        StepIndex = c.Int(nullable: false),
                        FormularsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Formular", t => t.FormularsId, cascadeDelete: true)
                .Index(t => t.FormularsId);
            
            CreateTable(
                "dbo.Falt",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        StegId = c.Int(nullable: false),
                        Kravs = c.Boolean(nullable: false),
                        Typ = c.Int(nullable: false),
                        Avgiftsbelagd = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WizardSteps", t => t.StegId, cascadeDelete: true)
                .Index(t => t.StegId);
            
            CreateTable(
                "dbo.Val",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypNamn = c.String(nullable: false),
                        Namn = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        FaltId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Falt", t => t.FaltId, cascadeDelete: true)
                .Index(t => t.FaltId);
            
            CreateTable(
                "dbo.FaltSvar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Varde = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        RegistreringsId = c.Int(nullable: false),
                        Falt_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Falt", t => t.Falt_Id, cascadeDelete: true)
                .ForeignKey("dbo.Registreringar", t => t.RegistreringsId, cascadeDelete: true)
                .Index(t => t.RegistreringsId)
                .Index(t => t.Falt_Id);
            
            AddColumn("dbo.Registreringar", "FormularsId", c => c.Int());
            AddColumn("dbo.Registreringar", "Formular_Id", c => c.Int());
            CreateIndex("dbo.Registreringar", "Formular_Id");
            AddForeignKey("dbo.Registreringar", "Formular_Id", "dbo.Formular", "Id");
            DropColumn("dbo.Registreringar", "EvenemangsId");
            DropColumn("dbo.Registreringar", "Lagnamn");
            DropColumn("dbo.Registreringar", "Startnummer");
            DropColumn("dbo.Registreringar", "Adress");
            DropColumn("dbo.Registreringar", "Telefon");
            DropColumn("dbo.Registreringar", "Klubb");
            DropColumn("dbo.Registreringar", "Bana_Id");
            DropColumn("dbo.Registreringar", "Kanot_Id");
            DropColumn("dbo.Registreringar", "Klass_Id");
            DropTable("dbo.Banor");
            DropTable("dbo.Kanoter");
            DropTable("dbo.Deltagare");
            DropTable("dbo.Klasser");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Klasser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Deltagare",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Förnamn = c.String(),
                        Efternamn = c.String(),
                        Personnummer = c.String(),
                        RegistreringarID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Kanoter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Banor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        AntalDeltagare = c.Int(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Registreringar", "Klass_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Registreringar", "Kanot_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Registreringar", "Bana_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Registreringar", "Klubb", c => c.String());
            AddColumn("dbo.Registreringar", "Telefon", c => c.String(nullable: false));
            AddColumn("dbo.Registreringar", "Adress", c => c.String(nullable: false));
            AddColumn("dbo.Registreringar", "Startnummer", c => c.Int(nullable: false));
            AddColumn("dbo.Registreringar", "Lagnamn", c => c.String(nullable: false));
            AddColumn("dbo.Registreringar", "EvenemangsId", c => c.Int(nullable: false));
            DropForeignKey("dbo.FaltSvar", "RegistreringsId", "dbo.Registreringar");
            DropForeignKey("dbo.FaltSvar", "Falt_Id", "dbo.Falt");
            DropForeignKey("dbo.Registreringar", "Formular_Id", "dbo.Formular");
            DropForeignKey("dbo.Formular", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.WizardSteps", "FormularsId", "dbo.Formular");
            DropForeignKey("dbo.Falt", "StegId", "dbo.WizardSteps");
            DropForeignKey("dbo.Val", "FaltId", "dbo.Falt");
            DropIndex("dbo.FaltSvar", new[] { "Falt_Id" });
            DropIndex("dbo.FaltSvar", new[] { "RegistreringsId" });
            DropIndex("dbo.Registreringar", new[] { "Formular_Id" });
            DropIndex("dbo.Val", new[] { "FaltId" });
            DropIndex("dbo.Falt", new[] { "StegId" });
            DropIndex("dbo.WizardSteps", new[] { "FormularsId" });
            DropIndex("dbo.Formular", new[] { "EvenemangsId" });
            DropColumn("dbo.Registreringar", "Formular_Id");
            DropColumn("dbo.Registreringar", "FormularsId");
            DropTable("dbo.FaltSvar");
            DropTable("dbo.Val");
            DropTable("dbo.Falt");
            DropTable("dbo.WizardSteps");
            DropTable("dbo.Formular");
            CreateIndex("dbo.Klasser", "EvenemangsId");
            CreateIndex("dbo.Deltagare", "RegistreringarID");
            CreateIndex("dbo.Registreringar", "Klass_Id");
            CreateIndex("dbo.Registreringar", "Kanot_Id");
            CreateIndex("dbo.Registreringar", "Bana_Id");
            CreateIndex("dbo.Registreringar", "EvenemangsId");
            CreateIndex("dbo.Kanoter", "EvenemangsId");
            CreateIndex("dbo.Banor", "EvenemangsId");
            AddForeignKey("dbo.Registreringar", "EvenemangsId", "dbo.Evenemang", "Id");
            AddForeignKey("dbo.Klasser", "EvenemangsId", "dbo.Evenemang", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Kanoter", "EvenemangsId", "dbo.Evenemang", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Registreringar", "Klass_Id", "dbo.Klasser", "Id");
            AddForeignKey("dbo.Registreringar", "Kanot_Id", "dbo.Kanoter", "Id");
            AddForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Registreringar", "Bana_Id", "dbo.Banor", "Id");
            AddForeignKey("dbo.Banor", "EvenemangsId", "dbo.Evenemang", "Id", cascadeDelete: true);
        }
    }
}
