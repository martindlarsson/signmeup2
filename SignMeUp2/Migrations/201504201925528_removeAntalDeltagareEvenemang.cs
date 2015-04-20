namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeAntalDeltagareEvenemang : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Evenemang", "AntalDeltagare");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Evenemang", "AntalDeltagare", c => c.Int(nullable: false));
        }
    }
}
