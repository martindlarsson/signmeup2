using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignMeUp2.Data
{
    public enum FaltTyp { text_falt = 0, val_falt = 1, epost_falt = 2, info_falt = 3 }

    [Table("Falt")]
    public partial class Falt
    {
        public Falt()
        {
            Val = new List<Val>();
        }

        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }

        public int? StegId { get; set; }
        public virtual FormularSteg Steg { get; set; }

        [Required]
        public bool Kravs { get; set; }

        public virtual ICollection<Val> Val { get; set; }

        [Required]
        public FaltTyp Typ { get; set; }

        [Required]
        public bool Avgiftsbelagd { get; set; }

        [Required]
        public int Index { get; set; }

        public virtual ICollection<ListaFalt> Listor { get; set; }
    }
}