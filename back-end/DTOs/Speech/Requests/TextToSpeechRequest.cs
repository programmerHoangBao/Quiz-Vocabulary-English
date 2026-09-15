using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Speech.Requests
{
    public class TextToSpeechRequest
    {
        [Required(ErrorMessage = "Text is required!")]
        public string Text { get; set; } = string.Empty;
    }
}
