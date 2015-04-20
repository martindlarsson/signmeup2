namespace SignMeUp2.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Registreringar")]
    public partial class Registreringar
    {
        public Registreringar()
        {
            Deltagare = new HashSet<Deltagare>();
        }

        public int ID { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        [StringLength(50)]
        public string Telefon { get; set; }

        [Required]
        public string Epost { get; set; }

        public bool Ranking { get; set; }

        public int Startnummer { get; set; }

        [Required]
        public string Lagnamn { get; set; }

        public int Kanot { get; set; }

        public string Klubb { get; set; }

        public int Klass { get; set; }

        public bool HarBetalt { get; set; }

        public int Forseningsavgift { get; set; }

        public DateTime Registreringstid { get; set; }

        public string Kommentar { get; set; }

        public int Bana { get; set; }

        public int Rabatter{ get; set; }

        public string PaysonToken { get; set; }

        public int Evenemang_Id { get; set; }

        public virtual Banor Banor { get; set; }

        public virtual ICollection<Deltagare> Deltagare { get; set; }

        public virtual Evenemang Evenemang { get; set; }

        public int Invoice { get; set; }

        public virtual Invoice Invoices { get; set; }

        public virtual Kanoter Kanoter { get; set; }

        public virtual Klasser Klasser { get; set; }
    }
}
