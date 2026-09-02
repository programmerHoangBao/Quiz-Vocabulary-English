using back_end.DTOs;
using back_end.DTOs.Auth.Requests;
using back_end.DTOs.Auth.Responses;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase // Fix: Inherit from ControllerBase to use StatusCode method
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var result = await _authService.RegisterAsync(req);
            return StatusCode(result.HttpStatusCode, result); // StatusCode is now accessible
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            var result = await _authService.VerifyOtpAsync(req);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var result = await _authService.LoginAsync(req);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("signin-google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest req)
        {
            var result = await _authService.GoogleLoginAsync(req.IdToken);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            var result = await _authService.RefreshTokenAsync(req);
            return StatusCode(result.HttpStatusCode, result);
        }
    }
}
