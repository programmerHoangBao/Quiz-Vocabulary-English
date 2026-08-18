namespace back_end.Configurations.Settings
{
    public class SecuritySetting
    {
        public const string SectionName = "Security";
        public string SecrectKey { get; set; } = string.Empty;
        public int OtpExpiryMinutes { get; set; } = 1;
        public string JwtSecretKey { get; set; } = string.Empty;

        public string JwtIssuer { get; set; } = string.Empty;

        public string JwtAudience { get; set; } = string.Empty;

        public int AccessTokenExpirationMinutes { get; set; } = 15;

        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
