using back_end.Enums;
using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Folder : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Private;
        public int CountLearn { get; set; } = 0;
        public string? imageUrl { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
