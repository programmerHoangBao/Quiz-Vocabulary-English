using back_end.DTOs.Auth.Requests;
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
    }
}
