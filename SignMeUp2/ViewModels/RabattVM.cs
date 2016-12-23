using System.ComponentModel.DataAnnotations;
using LangResources;

namespace SignMeUp2.ViewModels
{
    public class RabattVM
    {
        public int Id { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Language), ErrorMessage = Language.RequeiredFieldValError)]
        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        public string Kod { get; set; }

        //[Required(ErrorMessage = "Summa måste anges")]
        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        //[Range(typeof(int), "1", "99999999", ErrorMessage = "Summan måste vara ett positivt heltal")]
        [Range(typeof(int), "1", "99999999", ErrorMessageResourceName = "FieldValErrorPositiveNumber", ErrorMessageResourceType = typeof(Language))]
        public int Summa { get; set; }

        public string Beskrivning { get; set; }

        public int EvenemangsId { get; set; }
    }
}