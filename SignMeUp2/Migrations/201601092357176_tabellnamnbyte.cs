namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabellnamnbyte : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.WizardSteps", newName: "FormularsSteg");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.FormularsSteg", newName: "WizardSteps");
        }
    }
}
