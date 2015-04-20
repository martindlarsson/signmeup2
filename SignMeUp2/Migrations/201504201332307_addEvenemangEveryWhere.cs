namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEvenemangEveryWhere : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Banor", "Evenemang_Id", c => c.Int());
            AddColumn("dbo.Evenemang", "AntalDeltagare", c => c.Int(nullable: false));
            AddColumn("dbo.Kanoter", "Evenemang_Id", c => c.Int());
            AddColumn("dbo.Klasser", "Evenemang_Id", c => c.Int());
            CreateIndex("dbo.Banor", "Evenemang_Id");
            CreateIndex("dbo.Kanoter", "Evenemang_Id");
            CreateIndex("dbo.Klasser", "Evenemang_Id");
            AddForeignKey("dbo.Banor", "Evenemang_Id", "dbo.Evenemang", "Id");
            AddForeignKey("dbo.Kanoter", "Evenemang_Id", "dbo.Evenemang", "Id");
            AddForeignKey("dbo.Klasser", "Evenemang_Id", "dbo.Evenemang", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Klasser", "Evenemang_Id", "dbo.Evenemang");
            DropForeignKey("dbo.Kanoter", "Evenemang_Id", "dbo.Evenemang");
            DropForeignKey("dbo.Banor", "Evenemang_Id", "dbo.Evenemang");
            DropIndex("dbo.Klasser", new[] { "Evenemang_Id" });
            DropIndex("dbo.Kanoter", new[] { "Evenemang_Id" });
            DropIndex("dbo.Banor", new[] { "Evenemang_Id" });
            DropColumn("dbo.Klasser", "Evenemang_Id");
            DropColumn("dbo.Kanoter", "Evenemang_Id");
            DropColumn("dbo.Evenemang", "AntalDeltagare");
            DropColumn("dbo.Banor", "Evenemang_Id");
        }
    }
}
