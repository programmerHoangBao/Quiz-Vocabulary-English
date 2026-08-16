namespace back_end.Configurations.Settings
{
    public class SecuritySetting
    {
        public const string SectionName = "Security";
        public string SecrectKey { get; set; } = string.Empty;
        public int OtpExpiryMinutes { get; set; } = 1;
    }
}
