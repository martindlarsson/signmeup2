namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Banor")]
    public partial class Banor
    {
        public Banor()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Namn { get; set; }

        public int Avgift { get; set; }

        public int AntalDeltagare { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int Evenemang_ID { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
