namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class användarid : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Organisationer", "AnvändareId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organisationer", "AnvändareId", c => c.Int(nullable: false));
        }
    }
}
