namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormularStageNamn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormularSteg", "Namn", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormularSteg", "Namn");
        }
    }
}
