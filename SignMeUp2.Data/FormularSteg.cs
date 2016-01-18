using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    [Table("FormularSteg")]
    public partial class FormularSteg
    {
        public FormularSteg()
        {
            Falt = new List<Falt>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public int Index { get; set; }
        
        public int? FormularId { get; set; }
        public virtual Formular Formular { get; set; }

        public virtual ICollection<Falt> Falt { get; set; }
    }
}
