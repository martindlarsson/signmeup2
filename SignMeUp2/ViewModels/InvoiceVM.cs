using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public string Box { get; set; }

        [Required(ErrorMessage = "Postnummer måste anges")]
        public string Postnummer { get; set; }

        [Required(ErrorMessage = "Organisationsnummer måste anges")]
        public string Organisationsnummer { get; set; }

        [Required(ErrorMessage = "Postort måste anges")]
        public string Postort { get; set; }

        [Required(ErrorMessage = "Postadress måste anges")]
        public string Postadress { get; set; }

        [Required(ErrorMessage = "Företagsnamn måste anges")]
        [Display(Name = "Företagsnamn")]
        public string Namn { get; set; }

        public string Att { get; set; }
    }
}