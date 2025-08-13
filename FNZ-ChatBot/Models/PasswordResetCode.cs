using System.ComponentModel.DataAnnotations;

namespace FNZ_ChatBot.Models
{
    public class PasswordResetCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedAt { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;

        public bool IsValid => !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }
}