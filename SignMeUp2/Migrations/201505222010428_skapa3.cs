namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skapa3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Registreringar", name: "Forseningsavgift_Id", newName: "ForseningsavgiftId");
            RenameIndex(table: "dbo.Registreringar", name: "IX_Forseningsavgift_Id", newName: "IX_ForseningsavgiftId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Registreringar", name: "IX_ForseningsavgiftId", newName: "IX_Forseningsavgift_Id");
            RenameColumn(table: "dbo.Registreringar", name: "ForseningsavgiftId", newName: "Forseningsavgift_Id");
        }
    }
}
