namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListorKolumnordning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ListaFalt", "Index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ListaFalt", "Index");
        }
    }
}
