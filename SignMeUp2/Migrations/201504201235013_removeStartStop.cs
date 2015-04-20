namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeStartStop : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.StartOchSlut");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StartOchSlut",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Namn = c.String(nullable: false, maxLength: 50),
                        Datum = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
