namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttBetala : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registrering", "AttBetala", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Registrering", "AttBetala");
        }
    }
}
