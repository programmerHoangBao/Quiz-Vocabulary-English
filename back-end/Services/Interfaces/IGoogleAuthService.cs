using back_end.DTOs.Auth.Responses;

namespace back_end.Services.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> VerifyTokenAsync(string idToken);
    }
}
