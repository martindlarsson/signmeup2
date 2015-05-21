namespace SignMeUp2.Data
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

        public int? EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }

        // Allmänt
        [Required]
        public string Lagnamn { get; set; }

        [Required]
        public bool Ranking { get; set; }

        public int Startnummer { get; set; }

        [Required]
        public bool HarBetalt { get; set; }

        public string Kommentar { get; set; }

        // Kontaktuppgifter
        [Required]
        public string Adress { get; set; }

        [Required]
        public string Telefon { get; set; }

        [Required]
        public string Epost { get; set; }

        public string Klubb { get; set; }

        public string PaysonToken { get; set; }

        [Required]
        public DateTime? Registreringstid { get; set; }

        //public int? ForseningsavgiftsId { get; set; }
        public Forseningsavgift Forseningsavgift { get; set; }

        //public int? RabattId { get; set; }
        public Rabatter Rabatt { get; set; }

        public int BanId { get; set; }
        [Required]
        public Banor Bana { get; set; }

        public int KlassId { get; set; }
        [Required]
        public Klasser Klass { get; set; }

        public int KanotId { get; set; }
        [Required]
        public Kanoter Kanot { get; set; }

        //public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public ICollection<Deltagare> Deltagare { get; set; }
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
            //HasRequired(r => r.Bana)
            //    .WithMany(b => b.Registreringar)
            //    .HasForeignKey(e => e.BanId)
            //    .WillCascadeOnDelete(false);

            //HasRequired(r => r.Kanot)
            //    .WithMany(k => k.Registreringar)
            //    .HasForeignKey(e => e.KanotId)
            //    .WillCascadeOnDelete(false);

            //HasRequired(e => e.Klass)
            //    .WithMany(k => k.Registreringar)
            //    .HasForeignKey(e => e.KlassId)
            //    .WillCascadeOnDelete(false);

            HasRequired(r => r.Evenemang)
                .WithMany(e => e.Registreringar)
                .HasForeignKey(r => r.EvenemangsId)
                .WillCascadeOnDelete(false);

            HasOptional(r => r.Rabatt);
                //.WithMany(r => r.Registreringar)
                //.HasForeignKey(r => r.RabattId)
                //.WillCascadeOnDelete(false);

            HasOptional(r => r.Forseningsavgift);
                //.WithOptionalDependent(f => f.Registreringar)
                //.WillCascadeOnDelete(false);

            //HasOptional(r => r.Forseningsavgift)
            //    .WithMany(r => r.Registreringar)
            //    .HasForeignKey(r => r.ForseningsavgiftsId)
            //    .WillCascadeOnDelete(false);
        }
    }
}
