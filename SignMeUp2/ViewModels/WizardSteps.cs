using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SignMeUp2.Data;

namespace SignMeUp2.Models
{
    [Serializable]
    public class ContactViewModel : IWizardStep
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
    public class RegistrationViewModel : IWizardStep
    {
        [Required(ErrorMessage = "Lagnamn måste anges")]
        public string Lagnamn { get; set; }

        [Required(ErrorMessage = "Kanot måste väljas")]
        public int Kanot { get; set; }
        public virtual Kanoter Kanoter { get; set; }

        [Required(ErrorMessage = "Klass måste väljas")]
        public int Klass { get; set; }
        public virtual Klasser Klasser { get; set; }

        [Required(ErrorMessage = "Bana måste väljas")]
        public int Bana { get; set; }
        public virtual Banor Banor { get; set; }

        public bool Ranking { get; set; }
    }

    [Serializable]
    public class DeltagareListViewModel : IWizardStep
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

        [Required(ErrorMessage = "Efternamn måste anges")]
        public string Efternamn { get; set; }

        [Display(Name = "Personnr.")]
        public string Personnummer { get; set; }
    }

    //[Serializable]
    //public class BetalningssattViewModel
    //{
    //    public enum Betalningssätt {Faktura = 1, Payson = 2 }
    //    public Betalningssätt Betalningsmetod { get; set; }
    //}

    [Serializable]
    public class InvoiceViewModel
    {
        public string Box { get; set; }

        [Required(ErrorMessage = "Postnummer måste anges")]
        public string Postnummer { get; set; }

        [Required(ErrorMessage = "Organisationsnummer måste anges")]
        public string Organisationsnummer { get; set; }

        [Required(ErrorMessage = "Postort måste anges")]
        public string Postort { get; set; }

        [Required(ErrorMessage = "Postadress måste anges")]
        public string Postadress { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }

        public string Att { get; set; }
    }

}