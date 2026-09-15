using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.VocabularyProgress.Requests
{
    public class SubmitReviewRequest
    {
        [Required(ErrorMessage = "UserId is required!")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "VocabularyId is required!")]
        public Guid VocabularyId { get; set; }
        [Required(ErrorMessage = "ReviewMethod is required!")]
        public ReviewMethod Method { get; set; }
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
