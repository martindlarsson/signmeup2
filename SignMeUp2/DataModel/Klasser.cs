namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Klasser")]
    public partial class Klasser
    {
        public Klasser()
        {
            Registreringar = new HashSet<Registreringar>();
        }

        public int ID { get; set; }

        public string Namn { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public int Evenemang_ID { get; set; }
        public virtual Evenemang Evenemang { get; set; }
    }
}
