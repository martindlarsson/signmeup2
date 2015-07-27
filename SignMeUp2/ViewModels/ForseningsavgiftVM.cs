using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class ForseningsavgiftVM
    {
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public DateTime FranDatum { get; set; }

        [Required]
        public DateTime TillDatum { get; set; }

        [Required]
        public TypAvgift PlusEllerMinus { get; set; }

        [Required]
        public int Summa { get; set; }

    }
}
