using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Auth
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Otp code is required")]
        [RegularExpression(
            @"^\d{6}$",
            ErrorMessage = "OTP must be exactly 6 digits."
        )]
        public string OtpCode { get; set; } = string.Empty;
    }
}
