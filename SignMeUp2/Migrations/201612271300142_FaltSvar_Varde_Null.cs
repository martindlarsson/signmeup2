namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FaltSvar_Varde_Null : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FaltSvar", "Varde", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FaltSvar", "Varde", c => c.String(nullable: false));
        }
    }
}
