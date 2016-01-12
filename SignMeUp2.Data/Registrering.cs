namespace SignMeUp2.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Registrering")]
    public partial class Registrering
    {
        public int Id { get; set; }

        public int? FormularId { get; set; }
        public virtual Formular Formular { get; set; }

        public virtual ICollection<FaltSvar> Svar { get; set; }

        [Required]
        public bool HarBetalt { get; set; }

        public string Kommentar { get; set; }

        public string PaysonToken { get; set; }

        [Required]
        public DateTime? Registreringstid { get; set; }
        
        public int Forseningsavgift { get; set; }
        
        public int Rabatt { get; set; }
        public string Rabattkod { get; set; }

        public Fakturaadress Invoice { get; set; }
    }
}
