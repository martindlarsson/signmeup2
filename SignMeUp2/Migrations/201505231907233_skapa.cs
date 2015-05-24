namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skapa : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Evenemang",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        RegStart = c.DateTime(nullable: false),
                        RegStop = c.DateTime(nullable: false),
                        OrganisationsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisationer", t => t.OrganisationsId, cascadeDelete: true)
                .Index(t => t.OrganisationsId);
            
            CreateTable(
                "dbo.Forseningsavgift",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                        FranDatum = c.DateTime(nullable: false),
                        TillDatum = c.DateTime(nullable: false),
                        PlusEllerMinus = c.Int(nullable: false),
                        Summa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Kanoter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Registreringar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvenemangsId = c.Int(nullable: false),
                        Lagnamn = c.String(nullable: false),
                        Ranking = c.Boolean(nullable: false),
                        Startnummer = c.Int(nullable: false),
                        HarBetalt = c.Boolean(nullable: false),
                        Kommentar = c.String(),
                        Adress = c.String(nullable: false),
                        Telefon = c.String(nullable: false),
                        Epost = c.String(nullable: false),
                        Klubb = c.String(),
                        PaysonToken = c.String(),
                        Registreringstid = c.DateTime(nullable: false),
                        ForseningsavgiftId = c.Int(),
                        RabattId = c.Int(),
                        Bana_Id = c.Int(nullable: false),
                        Kanot_Id = c.Int(nullable: false),
                        Klass_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banor", t => t.Bana_Id)
                .ForeignKey("dbo.Forseningsavgift", t => t.ForseningsavgiftId)
                .ForeignKey("dbo.Kanoter", t => t.Kanot_Id)
                .ForeignKey("dbo.Klasser", t => t.Klass_Id)
                .ForeignKey("dbo.Rabatter", t => t.RabattId)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId)
                .Index(t => t.EvenemangsId)
                .Index(t => t.ForseningsavgiftId)
                .Index(t => t.RabattId)
                .Index(t => t.Bana_Id)
                .Index(t => t.Kanot_Id)
                .Index(t => t.Klass_Id);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registreringar", t => t.RegistreringarID, cascadeDelete: true)
                .Index(t => t.RegistreringarID);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Box = c.String(),
                        Postnummer = c.String(nullable: false),
                        Organisationsnummer = c.String(nullable: false),
                        Postort = c.String(nullable: false),
                        Postadress = c.String(nullable: false),
                        Namn = c.String(nullable: false),
                        Att = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registreringar", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Klasser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Rabatter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Kod = c.String(nullable: false),
                        Summa = c.Int(nullable: false),
                        Beskrivning = c.String(),
                        EvenemangsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Organisationer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        Epost = c.String(nullable: false),
                        Adress = c.String(nullable: false),
                        AnvändareId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Betalningsmetoder",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        GiroTyp = c.Int(nullable: false),
                        Gironummer = c.String(),
                        PaysonUserId = c.String(),
                        PaysonUserKey = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisationer", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrganisationsId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Registreringar", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Rabatter", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Evenemang", "OrganisationsId", "dbo.Organisationer");
            DropForeignKey("dbo.Betalningsmetoder", "Id", "dbo.Organisationer");
            DropForeignKey("dbo.Klasser", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Kanoter", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Registreringar", "RabattId", "dbo.Rabatter");
            DropForeignKey("dbo.Registreringar", "Klass_Id", "dbo.Klasser");
            DropForeignKey("dbo.Registreringar", "Kanot_Id", "dbo.Kanoter");
            DropForeignKey("dbo.Invoice", "Id", "dbo.Registreringar");
            DropForeignKey("dbo.Registreringar", "ForseningsavgiftId", "dbo.Forseningsavgift");
            DropForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar");
            DropForeignKey("dbo.Registreringar", "Bana_Id", "dbo.Banor");
            DropForeignKey("dbo.Forseningsavgift", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Banor", "EvenemangsId", "dbo.Evenemang");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Betalningsmetoder", new[] { "Id" });
            DropIndex("dbo.Rabatter", new[] { "EvenemangsId" });
            DropIndex("dbo.Klasser", new[] { "EvenemangsId" });
            DropIndex("dbo.Invoice", new[] { "Id" });
            DropIndex("dbo.Deltagare", new[] { "RegistreringarID" });
            DropIndex("dbo.Registreringar", new[] { "Klass_Id" });
            DropIndex("dbo.Registreringar", new[] { "Kanot_Id" });
            DropIndex("dbo.Registreringar", new[] { "Bana_Id" });
            DropIndex("dbo.Registreringar", new[] { "RabattId" });
            DropIndex("dbo.Registreringar", new[] { "ForseningsavgiftId" });
            DropIndex("dbo.Registreringar", new[] { "EvenemangsId" });
            DropIndex("dbo.Kanoter", new[] { "EvenemangsId" });
            DropIndex("dbo.Forseningsavgift", new[] { "EvenemangsId" });
            DropIndex("dbo.Evenemang", new[] { "OrganisationsId" });
            DropIndex("dbo.Banor", new[] { "EvenemangsId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Betalningsmetoder");
            DropTable("dbo.Organisationer");
            DropTable("dbo.Rabatter");
            DropTable("dbo.Klasser");
            DropTable("dbo.Invoice");
            DropTable("dbo.Deltagare");
            DropTable("dbo.Registreringar");
            DropTable("dbo.Kanoter");
            DropTable("dbo.Forseningsavgift");
            DropTable("dbo.Evenemang");
            DropTable("dbo.Banor");
        }
    }
}
