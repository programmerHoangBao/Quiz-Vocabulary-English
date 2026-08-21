using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Auth.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [MaxLength(20, ErrorMessage = "Password must not exceed 20 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
