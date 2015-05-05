namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    //using SignMeUp2.Models;

    [Table("Organisationer")]
    public class Organisation
    {
        public Organisation()
        {
            Evenemang = new HashSet<Evenemang>();
            //OrgBetalningar = new HashSet<OrgBetalningar>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        public string Namn { get; set; }

        [Required]
        public string Epost { get; set; }

        [Required]
        public string Adress { get; set; }

        public virtual ICollection<Evenemang> Evenemang { get; set; }

        public Betalningsmetoder Betalningsmetoder { get; set; }

        // TODO betalningar för att få använda tjänsten
        //public  virtual ICollection<OrgBetalningar> OrgBetalningar { get; set; }

        public string AnvändareId { get; set; }

        //public virtual ApplicationUser Användare { get; set; }

    }
}