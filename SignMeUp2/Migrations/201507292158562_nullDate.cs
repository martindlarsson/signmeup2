namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Evenemang", "FakturaBetaldSenast", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evenemang", "FakturaBetaldSenast", c => c.DateTime(nullable: false));
        }
    }
}
