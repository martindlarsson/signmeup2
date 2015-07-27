using SignMeUp2.Data;

namespace SignMeUp2.ViewModels
{
    public class FakturaVM
    {
        public string Evenemangsnamn { get; set; }

        public Registreringar Registrering { get; set; }

        public InvoiceViewModel Fakturaadress { get; set; }

        public Organisation Arrangor { get; set; }
    }
}