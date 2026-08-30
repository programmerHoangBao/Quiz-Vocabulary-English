using back_end.Configurations.Settings;
using back_end.DTOs.Auth.Responses;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;

namespace back_end.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleSetting _googleSetting;
        public GoogleAuthService(IOptions<GoogleSetting> options)
        {
            _googleSetting = options.Value;
        }

        public async Task<GoogleUserInfo?> VerifyTokenAsync(string idToken)
        {
            try
            {
                GoogleJsonWebSignature.Payload payload =
                    await GoogleJsonWebSignature.ValidateAsync(
                        idToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new[]
                            {
                                _googleSetting.ClientId
                            }
                        });

                return new GoogleUserInfo
                {
                    GoogleId = payload.Subject,
                    Email = payload.Email,
                    Name = payload.Name,
                    AvatarUrl = payload.Picture,
                    EmailVerified = payload.EmailVerified
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
