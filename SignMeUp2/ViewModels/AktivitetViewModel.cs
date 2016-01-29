using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class AktivitetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }
    }
}