namespace SignMeUp2.ViewModels
{
    public class FaltSvarVM
    {
        public int Id { get; set; }
        
        public int FaltId { get; set; }

        public FaltViewModel Falt { get; set; }
        
        public string Varde { get; set; }
        
        public int Avgift { get; set; }

        public int? RegistreringsId { get; set; }
    }
}