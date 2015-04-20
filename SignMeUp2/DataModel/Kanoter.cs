namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Kanoter")]
    public partial class Kanoter
    {
        public Kanoter()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        public string Namn { get; set; }

        public int? Avgift { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int Evenemang_ID { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
