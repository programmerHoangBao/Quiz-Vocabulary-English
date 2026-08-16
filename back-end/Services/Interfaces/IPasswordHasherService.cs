namespace back_end.Services.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
    }
}
