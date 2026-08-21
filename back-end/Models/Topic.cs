using back_end.Enums;
using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Topic : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Private;
        
        [Required]
        public Guid FolderId { get; set; }
        public Folder Folder { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Vocabolury> Vocaboluries { get; set; } = new List<Vocabolury>();
    }
}
