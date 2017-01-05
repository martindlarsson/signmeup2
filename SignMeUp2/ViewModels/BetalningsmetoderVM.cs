using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class BetalningsmetoderVM
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

        public int OrganisationsId { get; set; }
    }
}