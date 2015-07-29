namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BetalningsmetoderMm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evenemang", "Fakturabetalning", c => c.Boolean());
            AddColumn("dbo.Evenemang", "FakturaBetaldSenast", c => c.DateTime(nullable: false));
            AddColumn("dbo.Organisationer", "BildUrl", c => c.String());
            AddColumn("dbo.Betalningsmetoder", "HarPayson", c => c.Boolean(nullable: false));
            AddColumn("dbo.Betalningsmetoder", "KanTaEmotIntBetalningar", c => c.Boolean(nullable: false));
            AddColumn("dbo.Betalningsmetoder", "IBAN", c => c.String());
            AddColumn("dbo.Betalningsmetoder", "BIC", c => c.String());
            DropColumn("dbo.Registreringar", "Ranking");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Registreringar", "Ranking", c => c.Boolean(nullable: false));
            DropColumn("dbo.Betalningsmetoder", "BIC");
            DropColumn("dbo.Betalningsmetoder", "IBAN");
            DropColumn("dbo.Betalningsmetoder", "KanTaEmotIntBetalningar");
            DropColumn("dbo.Betalningsmetoder", "HarPayson");
            DropColumn("dbo.Organisationer", "BildUrl");
            DropColumn("dbo.Evenemang", "FakturaBetaldSenast");
            DropColumn("dbo.Evenemang", "Fakturabetalning");
        }
    }
}
