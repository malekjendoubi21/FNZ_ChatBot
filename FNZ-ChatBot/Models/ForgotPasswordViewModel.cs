using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;
    }
}