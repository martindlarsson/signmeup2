namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListorFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Listor", "Id", "dbo.Formular");
            DropForeignKey("dbo.ListaFalt", "ListaId", "dbo.Listor");
            DropIndex("dbo.Listor", new[] { "EvenemangId" });
            DropColumn("dbo.Listor", "FormularId");
            RenameColumn(table: "dbo.Listor", name: "Id", newName: "FormularId");
            RenameIndex(table: "dbo.Listor", name: "IX_Id", newName: "IX_FormularId");
            DropPrimaryKey("dbo.Listor");
            AlterColumn("dbo.Listor", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Listor", "EvenemangId", c => c.Int());
            AddPrimaryKey("dbo.Listor", "Id");
            CreateIndex("dbo.Listor", "EvenemangId");
            AddForeignKey("dbo.Listor", "FormularId", "dbo.Formular", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ListaFalt", "ListaId", "dbo.Listor", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ListaFalt", "ListaId", "dbo.Listor");
            DropForeignKey("dbo.Listor", "FormularId", "dbo.Formular");
            DropIndex("dbo.Listor", new[] { "EvenemangId" });
            DropPrimaryKey("dbo.Listor");
            AlterColumn("dbo.Listor", "EvenemangId", c => c.Int(nullable: false));
            AlterColumn("dbo.Listor", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Listor", "Id");
            RenameIndex(table: "dbo.Listor", name: "IX_FormularId", newName: "IX_Id");
            RenameColumn(table: "dbo.Listor", name: "FormularId", newName: "Id");
            AddColumn("dbo.Listor", "FormularId", c => c.Int(nullable: false));
            CreateIndex("dbo.Listor", "EvenemangId");
            AddForeignKey("dbo.ListaFalt", "ListaId", "dbo.Listor", "Id");
            AddForeignKey("dbo.Listor", "Id", "dbo.Formular", "Id");
        }
    }
}
