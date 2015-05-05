namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vetinte : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar");
            AddForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar");
            AddForeignKey("dbo.Deltagare", "RegistreringarID", "dbo.Registreringar", "ID");
        }
    }
}
