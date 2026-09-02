using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Auth.Requests
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required!")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
