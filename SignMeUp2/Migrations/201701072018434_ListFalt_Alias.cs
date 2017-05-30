namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListFalt_Alias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ListaFalt", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ListaFalt", "Alias");
        }
    }
}
