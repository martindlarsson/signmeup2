namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Registreringar")]
    public partial class Registreringar
    {
        public Registreringar()
        {
            Deltagare = new HashSet<Deltagare>();
        }

        public int Id { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }

        // Allmänt
        public string Lagnamn { get; set; }

        public bool Ranking { get; set; }

        public int Startnummer { get; set; }

        public bool HarBetalt { get; set; }

        public string Kommentar { get; set; }

        // Kontaktuppgifter
        public string Adress { get; set; }

        public string Telefon { get; set; }

        public string Epost { get; set; }

        public string Klubb { get; set; }

        public string PaysonToken { get; set; }

        public DateTime? Registreringstid { get; set; }

        public int ForseningsavgiftsId { get; set; }
        public virtual Forseningsavgift Forseningsavgift { get; set; }

        public int BanId { get; set; }
        public virtual Banor Bana { get; set; }

        public int RabattId { get; set; }
        public virtual Rabatter Rabatt { get; set; }

        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        public int KanotId { get; set; }
        public virtual Kanoter Kanot { get; set; }

        public int KlassId { get; set; }
        public virtual Klasser Klass { get; set; }

        public virtual ICollection<Deltagare> Deltagare { get; set; }
    }

    public class RegistreringarMap : EntityTypeConfiguration<Registreringar>
    {
        public RegistreringarMap()
        {
            // Key
            HasKey(e => e.Id);

            // Properties
            Property(r => r.Lagnamn).IsRequired();
            Property(r => r.Adress).IsRequired();
            Property(r => r.Epost).IsRequired();
            Property(r => r.HarBetalt);
            Property(r => r.Kommentar).IsOptional();
            Property(r => r.PaysonToken).IsOptional();
            Property(r => r.Ranking).IsOptional();
            Property(r => r.Registreringstid).IsRequired();
            Property(r => r.Startnummer).IsOptional();
            Property(r => r.Telefon).IsRequired();

            // Relatiionship
            HasRequired(r => r.Bana)
                .WithMany(b => b.Registreringar)
                .HasForeignKey(e => e.BanId)
                .WillCascadeOnDelete(false);

            HasRequired(r => r.Kanot)
                .WithMany(k => k.Registreringar)
                .HasForeignKey(e => e.KanotId)
                .WillCascadeOnDelete(false);

            HasRequired(e => e.Klass)
                .WithMany(k => k.Registreringar)
                .HasForeignKey(e => e.KlassId)
                .WillCascadeOnDelete(false);

            HasRequired(r => r.Evenemang)
                .WithMany(e => e.Registreringar)
                .HasForeignKey(e => e.EvenemangsId)
                .WillCascadeOnDelete(true);

            HasRequired(r => r.Rabatt)
                .WithMany(r => r.Registreringar)
                .HasForeignKey(r => r.RabattId)
                .WillCascadeOnDelete(false);

            HasRequired(r => r.Forseningsavgift)
                .WithMany(r => r.Registreringar)
                .HasForeignKey(r => r.ForseningsavgiftsId)
                .WillCascadeOnDelete(false);
        }
    }
}
