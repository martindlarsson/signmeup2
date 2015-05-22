namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skapa2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Registreringar", name: "BanId", newName: "Bana_Id");
            RenameColumn(table: "dbo.Registreringar", name: "KanotId", newName: "Kanot_Id");
            RenameColumn(table: "dbo.Registreringar", name: "KlassId", newName: "Klass_Id");
            RenameIndex(table: "dbo.Registreringar", name: "IX_BanId", newName: "IX_Bana_Id");
            RenameIndex(table: "dbo.Registreringar", name: "IX_KanotId", newName: "IX_Kanot_Id");
            RenameIndex(table: "dbo.Registreringar", name: "IX_KlassId", newName: "IX_Klass_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Registreringar", name: "IX_Klass_Id", newName: "IX_KlassId");
            RenameIndex(table: "dbo.Registreringar", name: "IX_Kanot_Id", newName: "IX_KanotId");
            RenameIndex(table: "dbo.Registreringar", name: "IX_Bana_Id", newName: "IX_BanId");
            RenameColumn(table: "dbo.Registreringar", name: "Klass_Id", newName: "KlassId");
            RenameColumn(table: "dbo.Registreringar", name: "Kanot_Id", newName: "KanotId");
            RenameColumn(table: "dbo.Registreringar", name: "Bana_Id", newName: "BanId");
        }
    }
}
