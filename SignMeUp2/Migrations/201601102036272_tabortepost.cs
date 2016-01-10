namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabortepost : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Registreringar", "Epost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Registreringar", "Epost", c => c.String(nullable: false));
        }
    }
}
