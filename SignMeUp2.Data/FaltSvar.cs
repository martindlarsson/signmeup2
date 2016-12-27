using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    [Table("FaltSvar")]
    public class FaltSvar
    {
        public int Id { get; set; }
        
        [Required]
        public int FaltId { get; set; }

        public Falt Falt { get; set; }
        
        public string Varde { get; set; }

        [Required]
        public int Avgift { get; set; }

        public int? RegistreringsId { get; set; }
        public virtual Registrering Registrering { get; set; }
    }
}
