namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Klasser")]
    public partial class Klasser
    {
        public Klasser()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int Id { get; set; }

        public string Namn { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }

    public class KlasserMap : EntityTypeConfiguration<Klasser>
    {
        public KlasserMap()
        {
            // Key
            HasKey(e => e.Id);

            // Properties
            Property(e => e.Namn).IsRequired();

            // Relatiionship
            HasRequired(k => k.Evenemang)
                .WithMany(e => e.Klasser)
                .HasForeignKey(k => k.EvenemangsId)
                .WillCascadeOnDelete(true);
        }
    }
}
