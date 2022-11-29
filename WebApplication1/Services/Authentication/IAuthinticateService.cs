using WebApplication1.Models.Entites;

namespace WebApplication1.Services.Authentication
{
    public interface IAuthinticateService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Users user);
        RefreshTokens GenerateRefreshToken(int userId);
        void SetRefreshToken(RefreshTokens newRefreshToken);
    }
}
