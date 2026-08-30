using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Auth.Requests
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "The id token of google is required!")]
        public string IdToken { get; set; } = string.Empty;
    }
}
