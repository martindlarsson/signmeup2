namespace SignMeUp2.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    public enum GiroTyp { Inget = 0, Postgiro = 1, Bankgiro = 2 }

    [Table("Betalningsmetoder")]
    public class Betalningsmetoder
    {
        public int Id { get; set; }

        public GiroTyp GiroTyp { get; set; }

        public string Gironummer { get; set; }

        public bool HarPayson { get; set; }

        //public string PaysonUserId { get; set; }

        //public string PaysonUserKey { get; set; }

        // Kan ta emot internationella betalningar
        public bool KanTaEmotIntBetalningar { get; set; }

        public string IBAN { get; set; }

        public string BIC { get; set; }

        public Organisation Organisation { get; set; }
    }
}