using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.Models
{
    [Serializable]
    public class WizardViewModel
    {
        public int CurrentStepIndex { get; set; }
        public int CountSteps { get { return Steps == null ? 0 : Steps.Count(); } }

        public IList<IWizardStep> Steps { get; set; }

        public int Evenemang_Id { get; set; }

        public Invoice Fakturaadress { get; set; }

        //public enum Betalningssätt { Payson = 1, Faktura = 2 }
        //public Betalningssätt Betalningsmetod { get; set; }

        public void Initialize()
        {
            Steps = new List<IWizardStep>();
            Steps.Add(new RegistrationViewModel());
            Steps.Add(new DeltagareListViewModel());
            Steps.Add(new ContactViewModel());
            CurrentStepIndex = 0;
        }

        public void UpdateSetp(IWizardStep step)
        {
            Steps[CurrentStepIndex] = step;
        }
    }
}