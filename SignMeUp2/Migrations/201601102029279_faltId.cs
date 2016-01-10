namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faltId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.FaltSvar", name: "Falt_Id", newName: "FaltId");
            RenameIndex(table: "dbo.FaltSvar", name: "IX_Falt_Id", newName: "IX_FaltId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.FaltSvar", name: "IX_FaltId", newName: "IX_Falt_Id");
            RenameColumn(table: "dbo.FaltSvar", name: "FaltId", newName: "Falt_Id");
        }
    }
}
