using back_end.Enums;

namespace back_end.DTOs.VocabularyProgress.Responses
{
    public class VocabularyProgressResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VocabularyId { get; set; }
        public VocabularyStatus Status { get; set; }
        public DateTime? NextReviewAt { get; set; }
        public int Score { get; set; }
        public DateTime Created { get; set; }
    }
}
