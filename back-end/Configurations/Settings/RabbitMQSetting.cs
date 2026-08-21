namespace back_end.Configurations.Settings
{
    public class RabbitMQSetting
    {
        public const string SectionName = "RabbitMQ";
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
