using System.ComponentModel.DataAnnotations;

namespace SignMeUp2.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Lösenordet måste vara minst 6 tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftande lösenordet stämmer inte överens.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-post måste anges")]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lösenord måste anges")]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "E-post måste anges")]
        [EmailAddress]
        [Display(Name = "Epost (användarnamn)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Löseord måste anges")]
        [StringLength(100, ErrorMessage = "Lösenordet måste vara minst 6 tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftande lösenordet stämmer inte överens.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        [Display(Name = "Organisationsnamn eller namn")]
        public string Organisation { get; set; }

        [Required(ErrorMessage = "Adress måste anges")]
        [Display(Name = "Adress")]
        public string Adress { get; set; }

        [EmailAddress]
        [Display(Name = "Epost (avsändare om annat än användarnamn)")]
        public string EmailSender { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Lösenordet måste vara minst 6 tecken långt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och det bekräftande lösenordet stämmer inte överens.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }
}
