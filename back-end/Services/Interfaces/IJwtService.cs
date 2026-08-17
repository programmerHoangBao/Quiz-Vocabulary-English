using back_end.DTOs.Projections;

namespace back_end.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(LoginUserProjection user);
    }
}
