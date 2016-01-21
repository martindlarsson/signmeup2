namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexOchAktiviteter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aktiviteter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Namn = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Annat')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Multisport')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('MTB')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Landsvägscykel')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Innebandy')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Orientering')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Triathlon')");
            Sql("INSERT INTO dbo.Aktiviteter VALUES ('Löpning')");

            AddColumn("dbo.Formular", "MaxRegistreringar", c => c.Int());
            AddColumn("dbo.Formular", "Publikt", c => c.Boolean(nullable: false, defaultValueSql: "1"));
            AddColumn("dbo.Formular", "AktivitetsId", c => c.Int(nullable: false, defaultValueSql: "1"));
            AddColumn("dbo.Falt", "Index", c => c.Int(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Val", "Index", c => c.Int(nullable: false, defaultValueSql: "0"));
            CreateIndex("dbo.Formular", "AktivitetsId");
            AddForeignKey("dbo.Formular", "AktivitetsId", "dbo.Aktiviteter", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Formular", "AktivitetsId", "dbo.Aktiviteter");
            DropIndex("dbo.Formular", new[] { "AktivitetsId" });
            DropColumn("dbo.Val", "Index");
            DropColumn("dbo.Falt", "Index");
            DropColumn("dbo.Formular", "AktivitetsId");
            DropColumn("dbo.Formular", "Publikt");
            DropColumn("dbo.Formular", "MaxRegistreringar");
            DropTable("dbo.Aktiviteter");
        }
    }
}
