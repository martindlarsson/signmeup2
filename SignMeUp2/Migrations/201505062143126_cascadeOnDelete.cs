namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Rabatter", "Evenemang_ID", "dbo.Evenemang");
            DropForeignKey("dbo.Registreringar", "Evenemang_Id", "dbo.Evenemang");
            AddForeignKey("dbo.Rabatter", "Evenemang_ID", "dbo.Evenemang", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Registreringar", "Evenemang_Id", "dbo.Evenemang", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registreringar", "Evenemang_Id", "dbo.Evenemang");
            DropForeignKey("dbo.Rabatter", "Evenemang_ID", "dbo.Evenemang");
            AddForeignKey("dbo.Registreringar", "Evenemang_Id", "dbo.Evenemang", "Id");
            AddForeignKey("dbo.Rabatter", "Evenemang_ID", "dbo.Evenemang", "Id");
        }
    }
}
