using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class RefreshToken : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string TokenHash { get; set; } = string.Empty;
        [Required]
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokeAt { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; } = null!;
    }
}
