using back_end.Enums;
using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class Vocabolury : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(255)]
        public string Word { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string Meaning { get; set; } = string.Empty;
        public PartOfSpeech? PartOfSpeech { get; set; }
        [MaxLength(255)]
        public string? ExampleEn { get; set; }
        [MaxLength(255)]
        public string? ExampleVn { get; set; }
        public string? IpaUk { get; set; }
        public string? IpaUs { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<VocabularyProgress> VocabularyProgresses { get; set; } = new List<VocabularyProgress>();
    }
}
