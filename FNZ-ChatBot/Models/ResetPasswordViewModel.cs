using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le code de vérification est requis.")]
        [Display(Name = "Code de vérification")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Le code doit contenir 6 caractères.")]
        public string VerificationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nouveau mot de passe est requis.")]
        [StringLength(100, ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le mot de passe et sa confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}