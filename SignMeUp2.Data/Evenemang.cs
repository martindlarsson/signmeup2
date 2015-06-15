namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Evenemang")]
    public partial class Evenemang
    {
        public Evenemang()
        {
            Registreringar = new HashSet<Registreringar>();
            Banor = new HashSet<Banor>();
            Kanoter = new HashSet<Kanoter>();
            Klasser = new HashSet<Klasser>();
            Rabatter = new HashSet<Rabatter>();
            Forseningsavgifter = new HashSet<Forseningsavgift>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegStart { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegStop { get; set; }

        public Organisation Organisation { get; set; }
        public int OrganisationsId { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public virtual ICollection<Banor> Banor { get; set; }

        public virtual ICollection<Kanoter> Kanoter { get; set; }

        public virtual ICollection<Klasser> Klasser { get; set; }

        public virtual ICollection<Rabatter> Rabatter { get; set; }

        public virtual ICollection<Forseningsavgift> Forseningsavgifter { get; set; }
    }

    //public class EvenemangMap : EntityTypeConfiguration<Evenemang>
    //{
    //    public EvenemangMap()
    //    {
    //        // Key
    //        HasKey(e => e.Id);

    //        // Properties
    //        Property(e => e.Namn).IsRequired();
    //        Property(e => e.RegStart).IsRequired();
    //        Property(e => e.RegStop).IsRequired();

    //        // Relatiionship
    //        HasRequired(e => e.Organisation)
    //            .WithMany(o => o.Evenemang)
    //            .HasForeignKey(e => e.OrganisationsId)
    //            .WillCascadeOnDelete(true);
    //    }
    //}
}
