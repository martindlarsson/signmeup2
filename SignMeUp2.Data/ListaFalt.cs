namespace SignMeUp2.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ListaFalt")]
    public partial class ListaFalt
    {
        public int Id { get; set; }

        [Required]
        public int Index { get; set; } // För att kunna välja ordning på kolumnerna i listan

        public int? FaltId { get; set; }
        public virtual Falt Falt { get; set; }

        public string Alias { get; set; }

        public int? ListaId { get; set; }
        public virtual Lista Lista { get; set; }
    }
}
