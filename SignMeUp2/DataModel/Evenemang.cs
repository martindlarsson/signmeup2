namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evenemang")]
    public partial class Evenemang
    {
        public Evenemang()
        {
            Registreringar = new HashSet<Registreringar>();
            Banor = new HashSet<Banor>();
            Kanoter = new HashSet<Kanoter>();
            Klasser = new HashSet<Klasser>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public DateTime RegStart { get; set; }

        [Required]
        public DateTime RegStop { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        public virtual ICollection<Banor> Banor { get; set; }

        public virtual ICollection<Kanoter> Kanoter { get; set; }

        public virtual ICollection<Klasser> Klasser { get; set; }

        public virtual ICollection<Rabatter> Rabatter { get; set; }
    }
}
