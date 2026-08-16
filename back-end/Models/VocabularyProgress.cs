using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models
{
    public class VocabularyProgress
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
        public int InCorrectCount { get; set; } = 0;
        public int Score { get; set; } = 0;
        public DateTime Created { get; set; } = DateTime.UtcNow; //Postgresql using DataTime.UtcNow
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; } = null!;
        public Vocabolury Vocabulary { get; set; } = null!;
    }
}
