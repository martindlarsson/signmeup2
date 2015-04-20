namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Klasser", "Evenemang_Id", "dbo.Evenemang");
            DropIndex("dbo.Banor", new[] { "Evenemang_Id" });
            DropIndex("dbo.Kanoter", new[] { "Evenemang_Id" });
            DropIndex("dbo.Klasser", new[] { "Evenemang_Id" });
            DropIndex("dbo.Rabatter", new[] { "Evenemang_Id" });
            AlterColumn("dbo.Banor", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Kanoter", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Klasser", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            AlterColumn("dbo.Rabatter", "Evenemang_ID", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Banor", "Evenemang_ID");
            CreateIndex("dbo.Kanoter", "Evenemang_ID");
            CreateIndex("dbo.Klasser", "Evenemang_ID");
            CreateIndex("dbo.Rabatter", "Evenemang_ID");
            AddForeignKey("dbo.Klasser", "Evenemang_ID", "dbo.Evenemang", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Klasser", "Evenemang_ID", "dbo.Evenemang");
            DropIndex("dbo.Rabatter", new[] { "Evenemang_ID" });
            DropIndex("dbo.Klasser", new[] { "Evenemang_ID" });
            DropIndex("dbo.Kanoter", new[] { "Evenemang_ID" });
            DropIndex("dbo.Banor", new[] { "Evenemang_ID" });
            AlterColumn("dbo.Rabatter", "Evenemang_ID", c => c.Int());
            AlterColumn("dbo.Klasser", "Evenemang_ID", c => c.Int());
            AlterColumn("dbo.Kanoter", "Evenemang_ID", c => c.Int());
            AlterColumn("dbo.Banor", "Evenemang_ID", c => c.Int());
            CreateIndex("dbo.Rabatter", "Evenemang_Id");
            CreateIndex("dbo.Klasser", "Evenemang_Id");
            CreateIndex("dbo.Kanoter", "Evenemang_Id");
            CreateIndex("dbo.Banor", "Evenemang_Id");
            AddForeignKey("dbo.Klasser", "Evenemang_Id", "dbo.Evenemang", "Id");
        }
    }
}
