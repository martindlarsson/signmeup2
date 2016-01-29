using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.ViewModels
{
    public class ListaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        public string Namn { get; set; }
        
        public int? FormularId { get; set; }
        public FormularViewModel Formular { get; set; }

        public ICollection<ListaFaltViewModel> Falt { get; set; }
    }
}