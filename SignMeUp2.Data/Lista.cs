namespace SignMeUp2.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Listor")]
    public partial class Lista
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Namn { get; set; }
        
        [Required]
        public int? FormularId { get; set; }
        public virtual Formular Formular { get; set; }

        public virtual ICollection<ListaFalt> Falt { get; set; }
    }
}
