using back_end.Configurations.Settings;
using back_end.DTOs;
using back_end.DTOs.Auth;
using back_end.Models;
using back_end.RabbitMQ.Interfaces;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;

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

        public AuthService(
            IUserRepository userRepository, 
            IPasswordHasherService passwordHasher, 
            IEmailService emailService,
            IOptions<AppSetting> appOptions,
            IOptions<SecuritySetting> securityOptions,
            IRabbitMqPublisher rabbitMqPublisher
        ) 
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _appSetting = appOptions.Value;
            _securitySetting = securityOptions.Value;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            int otpValue = random.Next(0, 1000000);
            return otpValue.ToString("D6");
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
