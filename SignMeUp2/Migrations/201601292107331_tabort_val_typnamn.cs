namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabort_val_typnamn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Val", "TypNamn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Val", "TypNamn", c => c.String(nullable: false));
        }
    }
}
