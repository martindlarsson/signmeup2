namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    public enum TypAvgift { Avgift = 0, Rabatt = 1}

    [Table("Forseningsavgift")]
    public partial class Forseningsavgift
    {
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        //[InverseProperty("Forseningsavgifter")]
        //[ForeignKey("EvenemangsId")]
        public Evenemang Evenemang { get; set; }
        public int? EvenemangsId { get; set; }

        [Required]
        public DateTime FranDatum { get; set; }

        [Required]
        public DateTime TillDatum { get; set; }

        [Required]
        public TypAvgift PlusEllerMinus { get; set; }

        [Required]
        public int Summa { get; set; }

        //public virtual ICollection<Registreringar> Registreringar { get; set; }
    }

    public class ForseningsavgiftMap : EntityTypeConfiguration<Forseningsavgift>
    {
        public ForseningsavgiftMap()
        {
            // Key
            HasKey(f => f.Id);

            // Properties
            Property(f => f.Namn).IsRequired();
            Property(f => f.FranDatum).IsRequired();
            Property(f => f.TillDatum).IsRequired();
            Property(f => f.PlusEllerMinus).IsRequired();
            Property(f => f.Summa).IsRequired();

            // Relatiionship
            HasRequired(f => f.Evenemang)
                .WithMany(e => e.Forseningsavgifter)
                .HasForeignKey(f => f.EvenemangsId)
                .WillCascadeOnDelete(true);
        }
    }
}
