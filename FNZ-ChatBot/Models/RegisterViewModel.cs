using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le pr�nom est requis")]
        [Display(Name = "Pr�nom")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Administrateur")]
        public bool IsAdmin { get; set; } = false;

        // Propri�t�s internes pour la g�n�ration automatique (non affich�es dans le formulaire)
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}