using System;
using System.ComponentModel.DataAnnotations;

using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class ForseningsavgiftVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Från datum måste anges")]
        public DateTime FranDatum { get; set; }

        [Required(ErrorMessage = "Till datum måste anges")]
        public DateTime TillDatum { get; set; }

        [Required(ErrorMessage = "Avgift/rabatt måste anges")]
        public TypAvgift PlusEllerMinus { get; set; }

        [Required(ErrorMessage = "Summa måste anges")]
        public int Summa { get; set; }

        public int EvenemangsId { get; set; }

    }
}
