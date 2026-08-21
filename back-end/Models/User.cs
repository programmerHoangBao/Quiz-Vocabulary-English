using back_end.Enums;
using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class User : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? GoogleId { get; set; }
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        [MaxLength(6)]
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsVerified { get; set; } = false;
        [Required]
        public RoleUser Role { get; set; } = RoleUser.User;
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
        public ICollection<VocabularyProgress> VocabularyProgresses { get; set; } = new List<VocabularyProgress>();
    }
}
