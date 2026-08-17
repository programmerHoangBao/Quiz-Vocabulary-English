using back_end.Enums;

namespace back_end.DTOs.Projections
{
    public class LoginUserProjection
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public RoleUser Role { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
