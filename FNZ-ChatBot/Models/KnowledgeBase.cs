using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class KnowledgeBase
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Réponse")]
        public string Response { get; set; } = string.Empty;
        
        [Display(Name = "Date de création")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Date de modification")]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Créé par")]
        public string CreatedBy { get; set; } = string.Empty;
        
        [Display(Name = "Modifié par")]
        public string ModifiedBy { get; set; } = string.Empty;
        
        [Display(Name = "Actif")]
        public bool IsActive { get; set; } = true;
    }
}