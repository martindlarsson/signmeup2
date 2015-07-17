//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;

//namespace SignMeUp2.ViewModels
//{
//    public class RegistrationViewModel : WizardStep
//    {

//        [Required(ErrorMessage = "Lagnamn måste anges")]
//        public string Lagnamn { get; set; }

//        [Required(ErrorMessage = "Kanot måste väljas")]
//        public int Kanot { get; set; }
//        public ValViewModel KanotVM { get; set; }

//        [Required(ErrorMessage = "Klass måste väljas")]
//        public int Klass { get; set; }
//        public ValViewModel KlassVM { get; set; }

//        [Required(ErrorMessage = "Bana måste väljas")]
//        public int Bana { get; set; }
//        public ValViewModel BanaVM { get; set; }
//    }
//}