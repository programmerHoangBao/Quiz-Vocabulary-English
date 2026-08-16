namespace back_end.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string name, string otp, int otpExpiryMinutes, string appName);
    }
}
