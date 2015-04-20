namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMoreOnEvenemang : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Banor", new[] { "Evenemang_Id" });
            DropIndex("dbo.Kanoter", new[] { "Evenemang_Id" });
            DropIndex("dbo.Klasser", new[] { "Evenemang_Id" });
            AddColumn("dbo.Rabatter", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Banor", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Kanoter", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Klasser", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Banor", "Evenemang_Id");
            CreateIndex("dbo.Kanoter", "Evenemang_Id");
            CreateIndex("dbo.Klasser", "Evenemang_Id");
            CreateIndex("dbo.Rabatter", "Evenemang_Id");
            AddForeignKey("dbo.Rabatter", "Evenemang_Id", "dbo.Evenemang", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rabatter", "Evenemang_Id", "dbo.Evenemang");
            DropIndex("dbo.Rabatter", new[] { "Evenemang_Id" });
            DropIndex("dbo.Klasser", new[] { "Evenemang_Id" });
            DropIndex("dbo.Kanoter", new[] { "Evenemang_Id" });
            DropIndex("dbo.Banor", new[] { "Evenemang_Id" });
            AlterColumn("dbo.Klasser", "Evenemang_ID", c => c.Int());
            AlterColumn("dbo.Kanoter", "Evenemang_ID", c => c.Int());
            AlterColumn("dbo.Banor", "Evenemang_ID", c => c.Int());
            DropColumn("dbo.Rabatter", "Evenemang_ID");
            CreateIndex("dbo.Klasser", "Evenemang_Id");
            CreateIndex("dbo.Kanoter", "Evenemang_Id");
            CreateIndex("dbo.Banor", "Evenemang_Id");
        }
    }
}
