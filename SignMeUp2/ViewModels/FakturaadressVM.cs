﻿using System.ComponentModel.DataAnnotations;
using LangResources;

namespace SignMeUp2.ViewModels
{
    public class FakturaadressVM
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "ZipCode", ResourceType = typeof(Language))]
        public string Postnummer { get; set; }

        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "OrgNumber", ResourceType = typeof(Language))]
        public string Organisationsnummer { get; set; }

        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "City", ResourceType = typeof(Language))]
        public string Postort { get; set; }
        
        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "Address", ResourceType = typeof(Language))]
        public string Postadress { get; set; }

        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "CompanyName", ResourceType = typeof(Language))]
        public string Namn { get; set; }

        [Required(ErrorMessageResourceName = "RequeiredFieldValError", ErrorMessageResourceType = typeof(Language))]
        [EmailAddress(ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(Language))]
        [Display(Name = "Email", ResourceType = typeof(Language))]
        public string Epost { get; set; }

        public int RegistreringsId { get; set; }
    }
}