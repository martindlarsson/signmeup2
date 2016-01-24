using System;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class EvenemangVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Datum måste anges")]
        [DataType(DataType.DateTime)]
        public DateTime RegStart { get; set; }

        [Required(ErrorMessage = "Datum måste anges")]
        [DataType(DataType.DateTime)]
        public DateTime RegStop { get; set; }

        public bool? Fakturabetalning { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FakturaBetaldSenast { get; set; }

        public int OrganisationsId { get; set; }
    }
}