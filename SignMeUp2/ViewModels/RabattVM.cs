using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.ViewModels
{
    public class RabattVM
    {
        public int Id { get; set; }

        [Required]
        public string Kod { get; set; }

        [Required]
        public int Summa { get; set; }

        public string Beskrivning { get; set; }
    }
}