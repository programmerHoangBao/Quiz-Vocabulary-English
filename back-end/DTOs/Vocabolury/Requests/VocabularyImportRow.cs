using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Vocabolury.Requests
{
    public class VocabularyImportRow
    {
        public string Word { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public string? PartOfSpeech { get; set; }

        public string? ExampleEn { get; set; }

        public string? ExampleVn { get; set; }

        public string? IpaUk { get; set; }

        public string? IpaUs { get; set; }

        //public string? ImageUrl { get; set; }
    }
}
