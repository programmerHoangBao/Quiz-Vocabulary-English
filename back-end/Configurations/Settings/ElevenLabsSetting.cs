namespace back_end.Configurations.Settings
{
    public class ElevenLabsSetting
    {
        public const string SectionName = "ElevenLabs";
        public string ApiKey { get; set; } = string.Empty;
        public string VoiceId { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;
    }
}
