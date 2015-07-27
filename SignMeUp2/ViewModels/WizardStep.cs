using System;
using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{
    public class WizardStep
    {
        public string Namn { get; set; }

        public int StepIndex { get; set; }
        public int StepCount { get; set; }

        public ICollection<FaltViewModel> FaltLista { get; set; }
    }
}