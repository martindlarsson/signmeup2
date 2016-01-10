namespace SignMeUp2.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Val")]
    public class Val
    {
        public int Id { get; set; }

        [Required]
        public string TypNamn { get; set; }

        [Required]
        public string Namn { get; set; }

        [Required]
        public int Avgift { get; set; }

        public int? FaltId { get; set; }
        public virtual Falt Falt { get; set; }
    }
}
