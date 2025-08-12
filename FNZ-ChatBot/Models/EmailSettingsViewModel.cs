using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class EmailSettingsViewModel
    {
        [Required]
        [Display(Name = "Nom de l'expéditeur")]
        public string FromName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email de l'expéditeur")]
        public string FromEmail { get; set; } = string.Empty;

        [Display(Name = "Utiliser un vrai serveur SMTP")]
        public bool UseRealSmtp { get; set; } = false;

        [Display(Name = "Serveur SMTP")]
        public string SmtpServer { get; set; } = string.Empty;

        [Display(Name = "Port")]
        public string Port { get; set; } = string.Empty;

        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Email de test")]
        [EmailAddress]
        public string TestEmail { get; set; } = string.Empty;
    }
}