using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Date de création")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Date de dernière activité")]
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<ConversationHistory> Messages { get; set; } = new List<ConversationHistory>();
    }
}