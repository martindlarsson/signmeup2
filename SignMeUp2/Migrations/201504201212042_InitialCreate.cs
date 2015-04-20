namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false, maxLength: 50),
                        Avgift = c.Int(nullable: false),
                        AntalDeltagare = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Registreringar",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Adress = c.String(nullable: false),
                        Telefon = c.String(nullable: false, maxLength: 50),
                        Epost = c.String(nullable: false),
                        Ranking = c.Boolean(nullable: false),
                        Startnummer = c.Int(nullable: false),
                        Lagnamn = c.String(nullable: false),
                        Kanot = c.Int(nullable: false),
                        Klubb = c.String(),
                        Klass = c.Int(nullable: false),
                        HarBetalt = c.Boolean(nullable: false),
                        Forseningsavgift = c.Int(nullable: false),
                        Registreringstid = c.DateTime(nullable: false),
                        Kommentar = c.String(),
                        Bana = c.Int(nullable: false),
                        Rabatter = c.Int(nullable: false),
                        PaysonToken = c.String(),
                        Evenemang_Id = c.Int(nullable: false),
                        Invoice = c.Int(nullable: false),
                        Invoices_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Evenemang", t => t.Evenemang_Id)
                .ForeignKey("dbo.Invoice", t => t.Invoices_Id)
                .ForeignKey("dbo.Kanoter", t => t.Kanot)
                .ForeignKey("dbo.Klasser", t => t.Klass)
                .ForeignKey("dbo.Banor", t => t.Bana)
                .Index(t => t.Kanot)
                .Index(t => t.Klass)
                .Index(t => t.Bana)
                .Index(t => t.Evenemang_Id)
                .Index(t => t.Invoices_Id);
            
            CreateTable(
                "dbo.Deltagare",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Förnamn = c.String(nullable: false),
                        Efternamn = c.String(nullable: false),
                        Personnummer = c.String(),
                        RegistreringarID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registreringar", t => t.RegistreringarID)
                .Index(t => t.RegistreringarID);
            
            CreateTable(
                "dbo.Evenemang",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        RegStart = c.DateTime(nullable: false),
                        RegStop = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Box = c.String(),
                        Postnummer = c.String(nullable: false),
                        Organisationsnummer = c.String(nullable: false),
                        Postort = c.String(nullable: false),
                        Postadress = c.String(nullable: false),
                        Namn = c.String(nullable: false),
                        Att = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Kanoter",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Namn = c.String(maxLength: 50),
                        Avgift = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Klasser",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Namn = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Forseningsavgift",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FranDatum = c.DateTime(nullable: false),
                        Summa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Rabatter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Kod = c.String(nullable: false),
                        Summa = c.Int(nullable: false),
                        Beskrivning = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StartOchSlut",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Namn = c.String(nullable: false, maxLength: 50),
                        Datum = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registreringar", "Bana", "dbo.Banor");
            DropForeignKey("dbo.Registreringar", "Klass", "dbo.Klasser");
            DropForeignKey("dbo.Registreringar", "Kanot", "dbo.Kanoter");
            DropForeignKey("dbo.Registreringar", "Invoices_Id", "dbo.Invoice");
            DropForeignKey("dbo.Registreringar", "Evenemang_Id", "dbo.Evenemang");
            DropForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar");
            DropIndex("dbo.Deltagare", new[] { "RegistreringarID" });
            DropIndex("dbo.Registreringar", new[] { "Invoices_Id" });
            DropIndex("dbo.Registreringar", new[] { "Evenemang_Id" });
            DropIndex("dbo.Registreringar", new[] { "Bana" });
            DropIndex("dbo.Registreringar", new[] { "Klass" });
            DropIndex("dbo.Registreringar", new[] { "Kanot" });
            DropTable("dbo.StartOchSlut");
            DropTable("dbo.Rabatter");
            DropTable("dbo.Forseningsavgift");
            DropTable("dbo.Klasser");
            DropTable("dbo.Kanoter");
            DropTable("dbo.Invoice");
            DropTable("dbo.Evenemang");
            DropTable("dbo.Deltagare");
            DropTable("dbo.Registreringar");
            DropTable("dbo.Banor");
        }
    }
}
