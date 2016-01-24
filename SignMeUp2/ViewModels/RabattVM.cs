using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class RabattVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kod måste anges")]
        public string Kod { get; set; }

        [Required(ErrorMessage = "Summa måste anges")]
        [Range(typeof(int), "1", "99999999", ErrorMessage = "Summan måste vara ett positivt heltal")]
        public int Summa { get; set; }

        public string Beskrivning { get; set; }

        public int EvenemangsId { get; set; }
    }
}