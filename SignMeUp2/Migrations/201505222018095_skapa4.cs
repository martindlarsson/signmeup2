namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skapa4 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Registreringar", name: "Rabatt_Id", newName: "RabattId");
            RenameIndex(table: "dbo.Registreringar", name: "IX_Rabatt_Id", newName: "IX_RabattId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Registreringar", name: "IX_RabattId", newName: "IX_Rabatt_Id");
            RenameColumn(table: "dbo.Registreringar", name: "RabattId", newName: "Rabatt_Id");
        }
    }
}
