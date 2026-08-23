using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Vocabolury.Requests
{
    public class CreateVocaboluryRequest
    {
        [Required(ErrorMessage = "Word is required!")]
        [MaxLength(255, ErrorMessage = "Word must not exceed 255 characters.")]
        public string Word { get; set; } = string.Empty;
        [Required(ErrorMessage = "Meaning is required!")]
        [MaxLength(255, ErrorMessage = "Meaning must not exceed 255 characters.")]
        public string Meaning { get; set; } = string.Empty;
        public PartOfSpeech? PartOfSpeech { get; set; }
        [MaxLength(255, ErrorMessage = "Example english must not exceed 255 characters.")]
        public string? ExampleEn { get; set; }
        [MaxLength(255, ErrorMessage = "Example english must not exceed 255 characters.")]
        public string? ExampleVn { get; set; }
        public string? IpaUk { get; set; }
        public string? IpaUs { get; set; }
        //public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        [Required(ErrorMessage = "Topic is required!")]
        public Guid TopicId { get; set; }
    }
}
