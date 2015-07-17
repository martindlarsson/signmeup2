//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using SignMeUp2.Data;
//using System.ComponentModel.DataAnnotations;

//namespace SignMeUp2.ViewModels
//{
//    [Serializable]
//    public class WizardViewModel
//    {
//        public WizardViewModel()
//        {
//            Steps = new List<IWizardStep>();
//            Steps.Add(new RegistrationViewModel());
//            Steps.Add(new DeltagareListViewModel());
//            Steps.Add(new KontaktuppgifterViewModel());
//            CurrentStepIndex = 0;
//        }

//        public int CurrentStepIndex { get; set; }
//        public int CountSteps { get { return Steps == null ? 0 : Steps.Count(); } }

//        public IList<IWizardStep> Steps { get; set; }

//        public int Evenemang_Id { get; set; }

//        public Invoice Fakturaadress { get; set; }

//        public Rabatter Rabatt { get; set; }

//        public string Rabattkod { get; set; }

//        public ForseningsavgiftVM Forseningsavgift { get; set; }

//        public BetalningViewModel Betalnignsposter { get; set; }

//        //public void Initialize()
//        //{
//        //    Steps = new List<IWizardStep>();
//        //    Steps.Add(new RegistrationViewModel());
//        //    Steps.Add(new DeltagareListViewModel());
//        //    Steps.Add(new ContactViewModel());
//        //    CurrentStepIndex = 0;
//        //}

//        public void UpdateSetp(IWizardStep step)
//        {
//            Steps[CurrentStepIndex] = step;
//        }

//        public RegistrationViewModel GetRegStep()
//        {
//            if (Steps == null)
//                return null;

//            return Steps.OfType<RegistrationViewModel>().FirstOrDefault();
//        }
//    }
//}