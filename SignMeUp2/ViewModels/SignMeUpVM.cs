﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

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

        public IList<WizardStep> Steps { get; set; }

        public WizardStep CurrentStep
        {
            get { return Steps[CurrentStepIndex]; }
        }

        public int EvenemangsId { get; set; }

        public string EvenemangsNamn { get; set; }

        public int RegistreringsId { get; set; }

        // Förseningsavgift eller kanpanj
        public ForseningsavgiftVM FAVM { get; set; }

        public string Rabattkod { get; set; }

        public RabattVM Rabatt { get; set; }

        public bool KanBetalaMedFaktura { get; set; }

        public IList<ValViewModel> Betalnignsposter
        {
            get
            {
                var list = new List<ValViewModel>();
                if (Steps != null)
                {
                    foreach (var step in Steps)
                    {
                        foreach (var falt in step.FaltLista)
                        {
                            if (falt.Avgiftsbelagd && falt.Typ == FaltTyp.val_falt)
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

        public WizardStep GetStep(string stegNamn)
        {
            return Steps.FirstOrDefault(s => s.Namn == stegNamn);
        }

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

    public class ValViewModel
    {
        public string TypNamn { get; set; }
        public int Id { get; set; }
        public string Namn { get; set; }
        public int Avgift { get; set; }
    }

    public class PaysonKontaktViewModel
    {
        [Display(Name = "Förnamn")]
        [Required(ErrorMessage = "Måste anges")]
        public string SenderFirstName { get; set; }

        [Display(Name = "Efternamn")]
        [Required(ErrorMessage = "Måste anges")]
        public string SenderLastName { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Detta är inte en välformad epostadress")]
        [Required(ErrorMessage = "Måste anges")]
        [Display(Name = "Epost")]
        [DataType(DataType.EmailAddress)]
        public string SenderEmail { get; set; }
    }
}