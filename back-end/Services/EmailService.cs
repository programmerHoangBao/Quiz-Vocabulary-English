using back_end.Configurations.Settings;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace back_end.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSetting _smtpSetting;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IOptions<SmtpSetting> smtpOptions, IWebHostEnvironment environment)
        {
            _smtpSetting = smtpOptions.Value;
            _environment = environment;
        }

        public async Task SendOtpEmailAsync(string toEmail, string name, string otp, int otpExpiryMinutes, string appName)
        {
            var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", "RegisterOtpTemplate.html");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found at path: {templatePath}");
            }
            var emailTemplate = await File.ReadAllTextAsync(templatePath);
            var emailBody = emailTemplate
                .Replace("{{Name}}", name)
                .Replace("{{OtpCode}}", otp)
                .Replace("{{OtpExpirationTime}}", otpExpiryMinutes.ToString())
                .Replace("{{AppName}}", appName);
            using var message = new MailMessage();
            message.From = new MailAddress(_smtpSetting.FromEmail, appName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "Verify Your Email - OTP Code";
            message.Body = emailBody;
            message.IsBodyHtml = true;
            using var client = new SmtpClient(_smtpSetting.Host, _smtpSetting.Port)
            {
                Credentials = new NetworkCredential(_smtpSetting.FromEmail, _smtpSetting.Password),
                EnableSsl = _smtpSetting.EnableSsl
            };
            await client.SendMailAsync(message);
        }
    }
}
