namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SwitchDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Registreringar", "Registreringstid", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Registreringar", "Registreringstid", c => c.DateTime(nullable: false));
        }
    }
}
