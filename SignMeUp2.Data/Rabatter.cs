namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Rabatter")]
    public partial class Rabatter
    {
        public int Id { get; set; }

        [Required]
        public string Kod { get; set; }

        [Required]
        public int Summa { get; set; }

        public string Beskrivning { get; set; }

        public virtual Evenemang Evenemang { get; set; }
        public int EvenemangsId { get; set; }

        //public virtual ICollection<Registreringar> Registreringar { get; set; }
    }

    public class RabattMap : EntityTypeConfiguration<Rabatter>
    {
        public RabattMap()
        {
            // Key
            HasKey(r => r.Id);

            // Properties
            Property(r => r.Kod).IsRequired();
            Property(r => r.Summa).IsRequired();
            Property(r => r.Beskrivning).IsOptional();

            // Relatiionship
            HasRequired(r => r.Evenemang)
                .WithMany(e => e.Rabatter)
                .HasForeignKey(r => r.EvenemangsId)
                .WillCascadeOnDelete(true);
        }
    }
}
