namespace back_end.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
