using AutoMapper;
using back_end.Configurations.Settings;
using back_end.DTOs;
using back_end.DTOs.Auth.Requests;
using back_end.DTOs.Auth.Responses;
using back_end.DTOs.Projections;
using back_end.Exceptions;
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
        private readonly AppSetting _appSetting;
        private readonly SecuritySetting _securitySetting;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IOptions<AppSetting> appOptions,
            IOptions<SecuritySetting> securityOptions,
            IRabbitMqPublisher rabbitMqPublisher,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IGoogleAuthService googleAuthService,
            IMapper mapper
        ) 
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _appSetting = appOptions.Value;
            _securitySetting = securityOptions.Value;
            _rabbitMqPublisher = rabbitMqPublisher;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _googleAuthService = googleAuthService;
            _mapper = mapper;
        }

        private static string GenerateOtp()
        {
            var random = new Random();
            int otpValue = random.Next(0, 1000000);
            return otpValue.ToString("D6");
        }

        public async Task<ApiResponse<LoginResponse>> GoogleLoginAsync(string idToken)
        {
            GoogleUserInfo? googleUser =
                await _googleAuthService.VerifyTokenAsync(idToken);
            if (googleUser == null)
            {
                throw new BusinessException(ErrorRecord.InvalidGoogleToken);
            }
            User? user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId);
            if (user == null)
            {
                user = await _userRepository.GetUserByEmailAsync(googleUser.Email);
                if (user != null)
                {
                    throw new BusinessException(ErrorRecord.UserExist);
                }
                user = new User
                {
                    Id = new Guid(),
                    Email = googleUser.Email,
                    Name = googleUser.Name,
                    GoogleId = googleUser.GoogleId,
                    AvatarUrl = googleUser.AvatarUrl,
                    AuthProvider = Enums.AuthProvider.Google,
                    IsVerified = true,
                    Role = Enums.RoleUser.User,
                    Password = null,
                    IsDeleted = false,
                };
                bool isCreted = await _userRepository.AddAsync(user);
                if (!isCreted)
                {
                    throw new BusinessException(ErrorRecord.RegisterFailed);
                }
            }
            LoginUserProjection loginUser = _mapper.Map<LoginUserProjection>(user);
            string accessToken = _jwtService.GenerateAccessToken(loginUser);
            string refreshToken = _refreshTokenService.GenerateToken();
            string refreshTokenHash = _refreshTokenService.HashToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                UserId = loginUser.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_securitySetting.RefreshTokenExpirationDays)
            };
            bool isSaveRefreshToken = await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            if (!isSaveRefreshToken)
            {
                throw new BusinessException(ErrorRecord.LoginFailed);
            }
            LoginResponse loginResponse = new LoginResponse
            {
                UserId = loginUser.Id,
                AccessToken = accessToken,
                ExpiresIn = _securitySetting.AccessTokenExpirationMinutes * 60,
            };
            return ApiResponse<LoginResponse>.MessageResponse(
                MessageRecord.LoginSuccess,
                loginResponse
            );
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest req)
        {
            LoginUserProjection? userLogin = await _userRepository.GetUserForLoginAsync(req.Email);
            if (userLogin == null) // Case 1: User not found!
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            else if (string.IsNullOrEmpty(userLogin.Password)) // Case 2: Password == Null
            {
                throw new BusinessException(ErrorRecord.LoginFailed);
            }
            // Case 3: Incorrect Password
            bool validPassword = _passwordHasher.VerifyPassword(req.Password, userLogin.Password);
            if (!validPassword)
            {
                throw new BusinessException(ErrorRecord.LoginFailed);
            }
            // Case 4: Login success
            string accessToken = _jwtService.GenerateAccessToken(userLogin);
            string refreshToken = _refreshTokenService.GenerateToken();
            string refreshTokenHash = _refreshTokenService.HashToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                UserId = userLogin.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_securitySetting.RefreshTokenExpirationDays)
            };
            bool isSaveRefreshToken = await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            if (!isSaveRefreshToken)
            {
                throw new BusinessException(ErrorRecord.LoginFailed);
            }
            LoginResponse loginResponse = new LoginResponse
            {
                UserId = userLogin.Id,
                AccessToken = accessToken,
                ExpiresIn = _securitySetting.AccessTokenExpirationMinutes * 60,
            };
            return ApiResponse<LoginResponse>.MessageResponse(
                MessageRecord.LoginSuccess, 
                loginResponse
            );
        }

        public async Task<ApiResponse<object?>> RegisterAsync(RegisterRequest req)
        { 
            User? existUser = await this._userRepository.GetUserByEmailAsync(req.Email);
            string otp = GenerateOtp();
            DateTime otpExpiry = DateTime.UtcNow.AddMinutes(_securitySetting.OtpExpiryMinutes);
            string hashedPassword = _passwordHasher.HashPassword(req.Password);
            // Case 1: User exists and isn't verified
            if (existUser != null && !existUser.IsVerified)
            {
                throw new BusinessException(ErrorRecord.UserIsNotVerify);
            }

            // Case 2: User exists and is verified
            if (existUser != null && existUser.IsVerified)
            {
                throw new BusinessException(ErrorRecord.UserExist);
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
                return ApiResponse<object?>.MessageResponse(MessageRecord.UserRegistered);
            }
            else
            {
                return ApiResponse<object?>.ErrorResponse(ErrorRecord.RegisterFailed);
            }
        }

        public async Task<ApiResponse<object?>> VerifyOtpAsync(VerifyOtpRequest req)
        {
            User? existUser = await this._userRepository.GetUserByEmailAsync(req.Email);
            // Case 1: User does not exist
            if (existUser == null)
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            else if (existUser.IsVerified) // Case 2: User exists but is already verified
            {
                throw new BusinessException(ErrorRecord.UserExist);
            }
            // Case 3: Otp is expired
            if (existUser.OtpExpiry < DateTime.UtcNow)
            {
                return ApiResponse<object?>.ErrorResponse(ErrorRecord.OtpExpiry);
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
                    return ApiResponse<object?>.MessageResponse(MessageRecord.VerifySuccess);
                }
            }
            throw new BusinessException(ErrorRecord.VerifyFailed);
        }
    }
}
