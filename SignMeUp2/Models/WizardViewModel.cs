using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignMeUp2.DataModel;

namespace SignMeUp2.Models
{
    [Serializable]
    public class WizardViewModel
    {
        public int CurrentStepIndex { get; set; }
        public int CountSteps { get { return Steps.Count(); } }

        public IList<IWizardStep> Steps { get; set; }

        public int Evenemang_Id { get; set; }

        public void Initialize()
        {
            Steps = new List<IWizardStep>();
            Steps.Add(new RegistrationStep());
            Steps.Add(new DeltagareStep());
            Steps.Add(new ContactStep());
            CurrentStepIndex = 0;
        }

        public void UpdateSetp(IWizardStep step)
        {
            Steps[CurrentStepIndex] = step;
        }
    }
}