namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fakturaadress_andringar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "Epost", c => c.String(nullable: false));
            DropColumn("dbo.Invoice", "Box");
            DropColumn("dbo.Invoice", "Att");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "Att", c => c.String());
            AddColumn("dbo.Invoice", "Box", c => c.String());
            DropColumn("dbo.Invoice", "Epost");
        }
    }
}
