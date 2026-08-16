namespace back_end.Configurations.Settings
{
    public class SmtpSetting
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
    }
}
