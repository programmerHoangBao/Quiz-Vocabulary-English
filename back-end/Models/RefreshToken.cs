using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string TokenHash { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokeAt { get; set; }
        public User User { get; set; } = null!;
    }
}
