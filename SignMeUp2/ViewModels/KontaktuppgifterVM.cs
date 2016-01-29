using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class KontaktuppgifterViewModel : FormularStegVM
    {
        // TODO namn på kontaktperson?

        [Required(ErrorMessage = "Adress måste anges")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Telefonnummer måste anges")]
        [StringLength(50)]
        public string Telefon { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Detta är inte en välformad epostadress")]
        [Required(ErrorMessage = "Epost måste anges")]
        [DataType(DataType.EmailAddress)]
        public string Epost { get; set; }

        public string Klubb { get; set; }
    }
}