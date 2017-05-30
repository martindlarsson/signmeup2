namespace SignMeUp2.ViewModels
{
    public class ListaFaltViewModel
    {
        public int Id { get; set; }
        
        public int Index { get; set; } // För att kunna välja ordning på kolumnerna i listan

        public int? FaltId { get; set; }
        public FaltViewModel Falt { get; set; }

        public string Alias { get; set; }

        public int? ListaId { get; set; }
        public ListaViewModel Lista { get; set; }
    }
}