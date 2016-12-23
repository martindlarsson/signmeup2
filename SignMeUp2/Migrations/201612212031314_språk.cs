namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class språk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evenemang", "Språk", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evenemang", "Språk");
        }
    }
}
