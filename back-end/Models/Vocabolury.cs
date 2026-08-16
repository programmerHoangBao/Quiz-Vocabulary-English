using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Vocabolury
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(255)]
        public string Word { get; set; }
        [Required]
        [MaxLength(255)]
        public string Meaning { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        [MaxLength(255)]
        public string? Example { get; set; }
        public string? IpaUk { get; set; }
        public string? IpaUs { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow; //Postgresql using DataTime.UtcNow
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<VocabularyProgress> VocabularyProgresses { get; set; } = new List<VocabularyProgress>();
    }
}
