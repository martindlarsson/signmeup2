using System.Collections.Generic;

namespace SignMeUp2.ViewModels
{
    public class FormularStegVM
    {
        public int Id { get; internal set; }

        public string Evenemang { get; set; }

        public string Formularsnamn { get; set; }

        public string Namn { get; set; }

        public int StepIndex { get; set; }
        public int StepCount { get; set; }

        public ICollection<FaltViewModel> FaltLista { get; set; }
    }
}