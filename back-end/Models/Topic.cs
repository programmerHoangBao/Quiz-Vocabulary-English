using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Topic
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string? Description { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Private;
        
        [Required]
        public Guid FolderId { get; set; }
        public Folder Folder { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow; //Postgresql using DataTime.UtcNow
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Vocabolury> Vocaboluries { get; set; } = new List<Vocabolury>();
    }
}
