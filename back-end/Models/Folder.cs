using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Folder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Private;

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow; //Postgresql using DataTime.UtcNow
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
