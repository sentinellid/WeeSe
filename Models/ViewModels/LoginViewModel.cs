using System.ComponentModel.DataAnnotations;

namespace WeeSe.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il nome utente è richiesto")]
        [Display(Name = "Nome Utente")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è richiesta")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }
}
