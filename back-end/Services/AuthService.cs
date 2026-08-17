using back_end.Configurations.Settings;
using back_end.DTOs;
using back_end.DTOs.Auth.Requests;
using back_end.DTOs.Auth.Responses;
using back_end.DTOs.Projections;
using back_end.Models;
using back_end.RabbitMQ.Interfaces;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities;

namespace back_end.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly AppSetting _appSetting;
        private readonly SecuritySetting _securitySetting;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository, 
            IPasswordHasherService passwordHasher, 
            IEmailService emailService,
            IOptions<AppSetting> appOptions,
            IOptions<SecuritySetting> securityOptions,
            IRabbitMqPublisher rabbitMqPublisher,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            IRefreshTokenRepository refreshTokenRepository
        ) 
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _appSetting = appOptions.Value;
            _securitySetting = securityOptions.Value;
            _rabbitMqPublisher = rabbitMqPublisher;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            int otpValue = random.Next(0, 1000000);
            return otpValue.ToString("D6");
        }

        public async Task<ApiResponse<object?>> LoginAsync(LoginRequest req)
        {
            LoginUserProjection userLogin = await _userRepository.GetUserForLoginAsync(req.Email);
            if (userLogin == null) // Case 1: User not found!
            {
                _logger.LogWarning("Login failed: User not found!");
                return ApiResponse<object?>.Response(MessageCode.LoginFailed);
            }
            else if (string.IsNullOrEmpty(userLogin.Password)) // Case 2: Password == Null
            {
                _logger.LogWarning("Login failed: User login with google,...");
                return ApiResponse<object?>.Response(MessageCode.LoginFailed);
            }
            // Case 3: Incorrect Password
            bool validPassword = _passwordHasher.VerifyPassword(req.Password, userLogin.Password);
            if (!validPassword)
            {
                _logger.LogWarning("Login failed: Incorrect password!");
                return ApiResponse<object?>.Response(MessageCode.LoginFailed);
            }
            // Case 4: Login success
            string accessToken = _jwtService.GenerateAccessToken(userLogin);
            string refreshToken = _refreshTokenService.GenerateToken();
            string refreshTokenHash = _refreshTokenService.HashToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                UserId = userLogin.Id,
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_securitySetting.RefreshTokenExpirationDays)
            };
            bool isSaveRefreshToken = await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            if (!isSaveRefreshToken)
            {
                _logger.LogWarning("Login failed: Save refresh token invalid!");
                return ApiResponse<object?>.Response(MessageCode.LoginFailed);
            }
            LoginResponse loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresIn = _securitySetting.AccessTokenExpirationMinutes * 60,
            };
            return ApiResponse<object?>.Response(MessageCode.LoginSuccess, loginResponse);
        }

        public async Task<ApiResponse<object?>> RegisterAsync(RegisterRequest req)
        { 
            User existUser = await this._userRepository.GetUserByEmailAsync(req.Email);
            string otp = GenerateOtp();
            DateTime otpExpiry = DateTime.UtcNow.AddMinutes(_securitySetting.OtpExpiryMinutes);
            string hashedPassword = _passwordHasher.HashPassword(req.Password);
            // Case 1: User exists and isn't verified
            if (existUser != null && !existUser.IsVerified)
            {
                return ApiResponse<object?>.Response(MessageCode.UserIsNotVerify);
            }

            // Case 2: User exists and is verified
            if (existUser != null && existUser.IsVerified)
            {
                return ApiResponse<object?>.Response(MessageCode.UserExist);
            }

            // Case 3: User does not exist, create a new user, isVerified is false, and send OTP email
            User newUser = new User
            {
                Email = req.Email,
                Password = hashedPassword,
                Name = req.Name,
                OtpCode = otp,
                OtpExpiry = otpExpiry
            };
            bool isAdded = await this._userRepository.AddAsync(newUser);
            if (isAdded)
            {
                // Send OTP email
                //await _emailService.SendOtpEmailAsync(req.Email, req.Name, otp, _securitySetting.OtpExpiryMinutes, _appSetting.Name);
                var message = new SendOtpMessage(
                    Email: req.Email,
                    OtpCode: otp,
                    Name: req.Name,
                    OtpExpiryMinutes: _securitySetting.OtpExpiryMinutes,
                    AppName: _appSetting.Name
                );
                await _rabbitMqPublisher.PublishAsync(message, "send_otp_email");
                return ApiResponse<object?>.Response(MessageCode.UserRegistered);
            }
            else
            {
                return ApiResponse<object?>.Response(MessageCode.RegisterFailed);
            }
        }

        public async Task<ApiResponse<object?>> VerifyOtpAsync(VerifyOtpRequest req)
        {
            User existUser = await this._userRepository.GetUserByEmailAsync(req.Email);
            // Case 1: User does not exist
            if (existUser == null)
            {
                return ApiResponse<object?>.Response(MessageCode.UserNotFound);
            }
            else if (existUser.IsVerified) // Case 2: User exists but is already verified
            {
                return ApiResponse<object?>.Response(MessageCode.UserExist);
            }
            // Case 3: Otp is expired
            if (existUser.OtpExpiry < DateTime.UtcNow)
            {
                return ApiResponse<object?>.Response(MessageCode.OtpExpiry);
            }
            // Case 4: Otp is valid and user is verified successfully
            if (existUser.OtpCode == req.OtpCode)
            {
                existUser.IsVerified = true;
                existUser.OtpCode = null; // Clear the OTP code after successful verification
                existUser.OtpExpiry = null; // Clear the OTP expiry after successful verification
                bool isUpdated = await this._userRepository.UpdateAsync(existUser);
                if (isUpdated)
                {
                    return ApiResponse<object?>.Response(MessageCode.VerifySuccess);
                }
            }
            return ApiResponse<object?>.Response(MessageCode.VerifyFailed);
        }
    }
}
