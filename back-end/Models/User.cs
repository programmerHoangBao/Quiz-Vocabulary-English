using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? GoogleId { get; set; }
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        [MaxLength(6)]
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
        public ICollection<VocabularyProgress> VocabularyProgresses { get; set; } = new List<VocabularyProgress>();
    }
}
