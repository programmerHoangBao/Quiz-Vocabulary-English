using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Vocabolury.Responses
{
    public class VocaboluryResponse
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public PartOfSpeech? PartOfSpeech { get; set; }
        public string? ExampleEn { get; set; }
        public string? ExampleVn { get; set; }
        public string? IpaUk { get; set; }
        public string? IpaUs { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Created { get; set; }
    }
}
