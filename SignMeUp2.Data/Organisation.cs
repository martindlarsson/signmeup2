namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.ModelConfiguration;

    [Table("Organisationer")]
    public class Organisation
    {
        public Organisation()
        {
            Evenemang = new HashSet<Evenemang>();
            //OrgBetalningar = new HashSet<OrgBetalningar>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public string Epost { get; set; }

        [Required]
        public string Adress { get; set; }

        public virtual ICollection<Evenemang> Evenemang { get; set; }

        public virtual Betalningsmetoder Betalningsmetoder { get; set; }

        // TODO betalningar för att få använda tjänsten
        //public  virtual ICollection<OrgBetalningar> OrgBetalningar { get; set; }

        public string AnvändareId { get; set; }

    }

    public class OrgMap : EntityTypeConfiguration<Organisation>
    {
        public OrgMap()
        {
            // Key
            HasKey(o => o.Id);

            // Properties
            Property(o => o.Namn).IsRequired();
            Property(o => o.Epost).IsRequired();
            Property(o => o.Adress).IsRequired();

            // Relatiionship
        }
    }
}