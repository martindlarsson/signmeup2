namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Kanoter")]
    public partial class Kanoter
    {
        public Kanoter()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public int Avgift { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }

    public class KanoterMap : EntityTypeConfiguration<Kanoter>
    {
        public KanoterMap()
        {
            // Key
            HasKey(e => e.Id);

            // Properties
            Property(e => e.Namn).IsRequired();
            Property(e => e.Avgift).IsRequired();

            // Relatiionship
            HasRequired(k => k.Evenemang)
                .WithMany(e => e.Kanoter)
                .HasForeignKey(k => k.EvenemangsId)
                .WillCascadeOnDelete(false);
        }
    }
}
