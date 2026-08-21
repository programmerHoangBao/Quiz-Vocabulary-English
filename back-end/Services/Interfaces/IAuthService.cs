using back_end.DTOs;
using back_end.DTOs.Auth.Requests;
using back_end.DTOs.Auth.Responses;

namespace back_end.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<object?>> RegisterAsync(RegisterRequest req);
        Task<ApiResponse<object?>> VerifyOtpAsync(VerifyOtpRequest req);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest req);
    }
}
