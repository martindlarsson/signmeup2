namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dynamiska_falt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Betalningsmetoder",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        GiroTyp = c.Int(nullable: false),
                        Gironummer = c.String(),
                        HarPayson = c.Boolean(nullable: false),
                        PaysonUserId = c.String(),
                        PaysonUserKey = c.String(),
                        KanTaEmotIntBetalningar = c.Boolean(nullable: false),
                        IBAN = c.String(),
                        BIC = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisation", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Organisation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        Epost = c.String(nullable: false),
                        Adress = c.String(nullable: false),
                        BildUrl = c.String(),
                        AnvändareId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Evenemang",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                        RegStart = c.DateTime(nullable: false),
                        RegStop = c.DateTime(nullable: false),
                        Fakturabetalning = c.Boolean(),
                        FakturaBetaldSenast = c.DateTime(),
                        OrganisationsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisation", t => t.OrganisationsId, cascadeDelete: true)
                .Index(t => t.OrganisationsId);
            
            CreateTable(
                "dbo.Formular",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvenemangsId = c.Int(nullable: false),
                        Namn = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangsId, cascadeDelete: true)
                .Index(t => t.EvenemangsId);
            
            CreateTable(
                "dbo.Registrering",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormularId = c.Int(nullable: false),
                        HarBetalt = c.Boolean(nullable: false),
                        Kommentar = c.String(),
                        PaysonToken = c.String(),
                        Registreringstid = c.DateTime(nullable: false),
                        Forseningsavgift = c.Int(nullable: false),
                        Rabatt = c.Int(nullable: false),
                        Rabattkod = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Formular", t => t.FormularId, cascadeDelete: true)
                .Index(t => t.FormularId);
            
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
                .ForeignKey("dbo.Registrering", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.FaltSvar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaltId = c.Int(nullable: false),
                        Varde = c.String(nullable: false),
                        Avgift = c.Int(nullable: false),
                        RegistreringsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Falt", t => t.FaltId)
                .ForeignKey("dbo.Registrering", t => t.RegistreringsId, cascadeDelete: true)
                .Index(t => t.FaltId)
                .Index(t => t.RegistreringsId);
            
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
                .ForeignKey("dbo.FormularSteg", t => t.StegId, cascadeDelete: true)
                .Index(t => t.StegId);
            
            CreateTable(
                "dbo.FormularSteg",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        FormularId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Formular", t => t.FormularId, cascadeDelete: true)
                .Index(t => t.FormularId);
            
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
            DropForeignKey("dbo.Evenemang", "OrganisationsId", "dbo.Organisation");
            DropForeignKey("dbo.Rabatter", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Forseningsavgift", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.Formular", "EvenemangsId", "dbo.Evenemang");
            DropForeignKey("dbo.FormularSteg", "FormularId", "dbo.Formular");
            DropForeignKey("dbo.Registrering", "FormularId", "dbo.Formular");
            DropForeignKey("dbo.FaltSvar", "RegistreringsId", "dbo.Registrering");
            DropForeignKey("dbo.FaltSvar", "FaltId", "dbo.Falt");
            DropForeignKey("dbo.Val", "FaltId", "dbo.Falt");
            DropForeignKey("dbo.Falt", "StegId", "dbo.FormularSteg");
            DropForeignKey("dbo.Invoice", "Id", "dbo.Registrering");
            DropForeignKey("dbo.Betalningsmetoder", "Id", "dbo.Organisation");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Rabatter", new[] { "EvenemangsId" });
            DropIndex("dbo.Forseningsavgift", new[] { "EvenemangsId" });
            DropIndex("dbo.Val", new[] { "FaltId" });
            DropIndex("dbo.FormularSteg", new[] { "FormularId" });
            DropIndex("dbo.Falt", new[] { "StegId" });
            DropIndex("dbo.FaltSvar", new[] { "RegistreringsId" });
            DropIndex("dbo.FaltSvar", new[] { "FaltId" });
            DropIndex("dbo.Invoice", new[] { "Id" });
            DropIndex("dbo.Registrering", new[] { "FormularId" });
            DropIndex("dbo.Formular", new[] { "EvenemangsId" });
            DropIndex("dbo.Evenemang", new[] { "OrganisationsId" });
            DropIndex("dbo.Betalningsmetoder", new[] { "Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Rabatter");
            DropTable("dbo.Forseningsavgift");
            DropTable("dbo.Val");
            DropTable("dbo.FormularSteg");
            DropTable("dbo.Falt");
            DropTable("dbo.FaltSvar");
            DropTable("dbo.Invoice");
            DropTable("dbo.Registrering");
            DropTable("dbo.Formular");
            DropTable("dbo.Evenemang");
            DropTable("dbo.Organisation");
            DropTable("dbo.Betalningsmetoder");
        }
    }
}
