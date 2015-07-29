namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Registreringar")]
    public partial class Registreringar
    {
        public Registreringar()
        {
            Deltagare = new HashSet<Deltagare>();
        }

        public int Id { get; set; }

        public int? EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }

        // Allmänt
        [Required]
        public string Lagnamn { get; set; }
        
        public int Startnummer { get; set; }

        [Required]
        public bool HarBetalt { get; set; }

        public string Kommentar { get; set; }

        // Kontaktuppgifter
        [Required]
        public string Adress { get; set; }

        [Required]
        public string Telefon { get; set; }

        [Required]
        public string Epost { get; set; }

        public string Klubb { get; set; }

        public string PaysonToken { get; set; }

        [Required]
        public DateTime? Registreringstid { get; set; }

        public Forseningsavgift Forseningsavgift { get; set; }
        public int? ForseningsavgiftId { get; set; }

        public Rabatter Rabatt { get; set; }
        public int? RabattId { get; set; }

        [Required]
        public Banor Bana { get; set; }

        [Required]
        public Klasser Klass { get; set; }

        [Required]
        public Kanoter Kanot { get; set; }

        public Invoice Invoice { get; set; }

        public ICollection<Deltagare> Deltagare { get; set; }
    }
}
