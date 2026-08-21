using back_end.Enums;
using back_end.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class VocabularyProgress : IAuditable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid VocabularyId { get; set; }

        public VocabularyStatus Status { get; set; } = VocabularyStatus.Learning;
        public DateTime? NextReviewAt { get; set; }
        public int ReviewCount { get; set; } = 0;
        public int CorrectCount { get; set; } = 0;
        public int IncorrectCount { get; set; } = 0;
        public int Score { get; set; } = 0;
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; } = null!;
        public Vocabolury Vocabulary { get; set; } = null!;
    }
}
