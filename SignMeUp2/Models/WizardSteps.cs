using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SignMeUp2.DataModel;

namespace SignMeUp2.Models
{
    [Serializable]
    public class ContactStep : IWizardStep
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

    [Serializable]
    public class RegistrationStep : IWizardStep
    {
        [Required(ErrorMessage = "Lagnamn måste anges")]
        public string Lagnamn { get; set; }

        [Required(ErrorMessage = "Kanot måste väljas")]
        public int Kanot { get; set; }

        [Required(ErrorMessage = "Klass måste väljas")]
        public int Klass { get; set; }

        [Required(ErrorMessage = "Bana måste väljas")]
        public int Bana { get; set; }

        public bool Ranking { get; set; }
    }

    [Serializable]
    public class DeltagareStep : IWizardStep
    {
        public IList<DeltagareViewModel> DeltagareLista { get; set; }
        public bool KravPersonnummer { get; set; }
        private int _AntalDeltagareBana;
        public int AntalDeltagareBana
        {
            get { return _AntalDeltagareBana; }
            set
            {
                _AntalDeltagareBana = value;
                if (DeltagareLista == null || DeltagareLista.Count() != _AntalDeltagareBana)
                {
                    DeltagareLista = new List<DeltagareViewModel>();
                    for (int i = 0; i < _AntalDeltagareBana; i++)
                    {
                        DeltagareLista.Add(new DeltagareViewModel());
                    }
                }
            }
        }
    }

    [Serializable]
    public class DeltagareViewModel
    {
        [Required(ErrorMessage = "Förnamn måste anges")]
        public string Förnamn { get; set; }

        [Required(ErrorMessage = "Förnamn måste anges")]
        public string Efternamn { get; set; }

        public string Personnummer { get; set; }
    }

}