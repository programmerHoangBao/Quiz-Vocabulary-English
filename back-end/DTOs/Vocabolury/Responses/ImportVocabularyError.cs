namespace back_end.DTOs.Vocabolury.Responses
{
    public class ImportVocabularyError
    {
        public int Row { get; set; }

        public string Word { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;
    }
}
