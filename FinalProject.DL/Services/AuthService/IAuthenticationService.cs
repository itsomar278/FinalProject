using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;

namespace WebApplication1.Services.AuthService
{
    public interface IAuthenticationService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Users user);
        Task<string> RefreshToken(UserSessionModel sessionUser, string refreshToken);
        Task<string> login(UserLoginRequest request);
        Task<ActionResult> logout(UserSessionModel user);
        Task<ActionResult> Register(UserRegisterRequest request);
        Task<ActionResult> UpdatePassword(UserPasswordUpdateRequest request , UserSessionModel user);
        Task<ActionResult> UpdateUserName(UpdateUserNameRequest request, UserSessionModel user);
        Task<RefreshTokens> GenerateRefreshToken(string email);
        Task UpdateUserRefreshToken(string userEmail, RefreshTokens refreshToken);
    }
}
