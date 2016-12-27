using System.Collections.Generic;
using System.Linq;
using LangResources;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class SignMeUpVM
    {
        public SignMeUpVM()
        {
            CurrentStepIndex = 0;
        }

        public int CurrentStepIndex { get; set; }
        public int CountSteps { get { return Steps == null ? 0 : Steps.Count(); } }

        public FormularViewModel Formular { get; set; }

        public List<FormularStegVM> Steps { get { return Formular.Steg.ToList(); } }

        public FormularStegVM CurrentStep
        {
            
            get {
                if (CurrentStepIndex < 0)
                {
                    CurrentStepIndex = 0;
                    return Steps[0];
                }
                return Steps[CurrentStepIndex];
            }
        }

        public int FormularsId { get; set; }

        public int EvenemangsId { get; set; }

        public string EvenemangsNamn { get; set; }

        public Data.Språk EvenemangsSpråk { get; set; }

        public int RegistreringsId { get; set; }

        // Förseningsavgift eller kanpanj
        public ForseningsavgiftVM FAVM { get; set; }

        [Display(Name = "DiscountCode", ResourceType = typeof(Language))]
        public string Rabattkod { get; set; }

        public RabattVM Rabatt { get; set; }

        public bool KanBetalaMedFaktura { get; set; }

        public IList<ValViewModel> Betalnignsposter
        {
            get
            {
                var list = new List<ValViewModel>();

                // Lägg till grundavgiften för evenemanget
                if (Formular.Avgift != 0)
                {
                    list.Add(new ValViewModel
                    {
                        Avgift = Formular.Avgift,
                        Id = Formular.Id,
                        Namn = Formular.Namn,
                        TypNamn = "Grundavgift"
                    });
                }   

                if (Steps != null)
                {
                    foreach (var step in Steps)
                    {
                        foreach (var falt in step.FaltLista)
                        {
                            if (falt.Avgiftsbelagd && falt.Typ == Data.FaltTyp.val_falt)
                            {
                                list.Add(falt.Val.FirstOrDefault(v => v.Id == int.Parse(falt.Varde)));
                            }
                        }
                    }
                }

                // Rabatt
                if (Rabatt != null)
                {
                    list.Add(new ValViewModel { TypNamn = "Rabatt", Namn = Rabatt.Kod, Avgift = -Rabatt.Summa });
                }

                // Förseningsavgift
                if (FAVM != null)
                {
                    var summa = FAVM.PlusEllerMinus == Data.TypAvgift.Avgift ? FAVM.Summa : -FAVM.Summa;
                    list.Add(new ValViewModel { TypNamn = FAVM.PlusEllerMinus.ToString(), Namn = FAVM.Namn, Avgift = summa });
                }

                return list;
            }
        }

        public int AttBetala
        {
            get {
                int summa = 0;
                foreach (var val in Betalnignsposter)
                {
                    summa += val.Avgift;
                }
                return summa < 0 ? 0 : summa;
            }
        }

        public InvoiceViewModel Fakturaadress { get; set; }

        public PaysonKontaktViewModel Kontaktinformation { get; set; }

        public string PaysonToken { get; set; }
        
        public string GetFaltvarde(string faltnamn)
        {
            foreach (var steg in Steps)
            {
                foreach (var falt in steg.FaltLista)
                {
                    if (falt.Namn == faltnamn)
                    {
                        return falt.Varde;
                    }
                }
            }

            return null;
        }
    }

    public class PaysonKontaktViewModel
    {
        [Display(Name = "FirstName", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ValidationRequiredField", ErrorMessageResourceType = typeof(Language))]
        public string SenderFirstName { get; set; }
        
        [Display(Name = "SirName", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ValidationRequiredField", ErrorMessageResourceType = typeof(Language))]
        public string SenderLastName { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Detta är inte en välformad epostadress")]
        [Required(ErrorMessageResourceName = "ValidationRequiredField", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "Email", ResourceType = typeof(Language))]
        [DataType(DataType.EmailAddress)]
        public string SenderEmail { get; set; }
    }
}