using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    [Table("Formular")]
    public partial class Formular
    {
        public int Id { get; set; }

        public int? EvenemangsId { get; set; }
        public virtual Evenemang Evenemang { get; set; }

        public virtual ICollection<Registreringar> Registreringar { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public bool Gratis { get; set; }

        [Required]
        public int Startavgift { get; set; }
        
        public virtual ICollection<WizardStep> Steg { get; set; }
    }
}
