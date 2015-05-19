namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Banor")]
    public partial class Banor
    {
        public Banor()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int Id { get; set; }

        public string Namn { get; set; }

        public int Avgift { get; set; }

        public int AntalDeltagare { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }

    public class BanorMap : EntityTypeConfiguration<Banor>
    {
        public BanorMap()
        {
            // Key
            HasKey(e => e.Id);

            // Properties
            Property(e => e.Namn).IsRequired();
            Property(e => e.Avgift).IsRequired();
            Property(e => e.AntalDeltagare).IsRequired();

            // Relatiionship
            HasRequired(b => b.Evenemang)
                .WithMany(e => e.Banor)
                .HasForeignKey(b => b.EvenemangsId)
                .WillCascadeOnDelete(true);
        }
    }
}
