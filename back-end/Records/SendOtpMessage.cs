namespace back_end.Records
{
    public record SendOtpMessage(
        string Email, 
        string OtpCode,
        string Name,
        int OtpExpiryMinutes,
        string AppName
    )
    {
    }
}
