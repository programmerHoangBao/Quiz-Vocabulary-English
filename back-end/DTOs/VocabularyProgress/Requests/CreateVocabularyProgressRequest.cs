using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.VocabularyProgress.Requests
{
    public class CreateVocabularyProgressRequest
    {
        [Required(ErrorMessage = "UserId is required!")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "VocabularyId is required!")]
        public Guid VocabularyId { get; set; }
    }
}
