namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tagitbortpaysonfalt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Betalningsmetoder", "PaysonUserId");
            DropColumn("dbo.Betalningsmetoder", "PaysonUserKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Betalningsmetoder", "PaysonUserKey", c => c.String());
            AddColumn("dbo.Betalningsmetoder", "PaysonUserId", c => c.String());
        }
    }
}
