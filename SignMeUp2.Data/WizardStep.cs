using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    [Table("FormularsSteg")]
    public partial class WizardStep
    {
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public int StepIndex { get; set; }
        
        public int? FormularsId { get; set; }
        public virtual Formular Formular { get; set; }

        public virtual ICollection<Falt> Falt { get; set; }
    }
}
