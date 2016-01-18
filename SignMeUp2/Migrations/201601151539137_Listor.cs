namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Listor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ListaFalt",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaltId = c.Int(nullable: false),
                        ListaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Listor", t => t.ListaId)
                .ForeignKey("dbo.Falt", t => t.FaltId, cascadeDelete: true)
                .Index(t => t.FaltId)
                .Index(t => t.ListaId);
            
            CreateTable(
                "dbo.Listor",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        EvenemangId = c.Int(nullable: false),
                        Namn = c.String(nullable: false),
                        FormularId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Formular", t => t.Id)
                .ForeignKey("dbo.Evenemang", t => t.EvenemangId)
                .Index(t => t.Id)
                .Index(t => t.EvenemangId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Listor", "EvenemangId", "dbo.Evenemang");
            DropForeignKey("dbo.ListaFalt", "FaltId", "dbo.Falt");
            DropForeignKey("dbo.Listor", "Id", "dbo.Formular");
            DropForeignKey("dbo.ListaFalt", "ListaId", "dbo.Listor");
            DropIndex("dbo.Listor", new[] { "EvenemangId" });
            DropIndex("dbo.Listor", new[] { "Id" });
            DropIndex("dbo.ListaFalt", new[] { "ListaId" });
            DropIndex("dbo.ListaFalt", new[] { "FaltId" });
            DropTable("dbo.Listor");
            DropTable("dbo.ListaFalt");
        }
    }
}
