namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListorFix2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Listor", "EvenemangId", "dbo.Evenemang");
            DropIndex("dbo.Listor", new[] { "EvenemangId" });
            DropColumn("dbo.Listor", "EvenemangId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Listor", "EvenemangId", c => c.Int());
            CreateIndex("dbo.Listor", "EvenemangId");
            AddForeignKey("dbo.Listor", "EvenemangId", "dbo.Evenemang", "Id");
        }
    }
}
