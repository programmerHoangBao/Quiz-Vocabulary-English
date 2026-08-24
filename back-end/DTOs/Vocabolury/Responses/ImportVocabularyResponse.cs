namespace back_end.DTOs.Vocabolury.Responses
{
    public class ImportVocabularyResponse
    {
        public int TotalRows { get; set; }

        public int SuccessCount { get; set; }

        public int FailedCount { get; set; }

        public List<ImportVocabularyError> Errors { get; set; } = new();
    }
}
