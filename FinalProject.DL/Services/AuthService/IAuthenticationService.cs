using DataAcess.Entites;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Services.SessionService;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Services.AuthService
{
    public interface IAuthenticationService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Users user);
        Task<string> RefreshToken(UserSessionModel sessionUser, string refreshToken);
        Task<string> login(UserLoginRequestDto request);
        Task<ActionResult> logout(UserSessionModel user);
        Task<ActionResult> Register(UserRegisterRequestDto request);
        Task<ActionResult> UpdatePassword(UserPasswordUpdateRequestDto request, UserSessionModel user);
        Task<ActionResult> UpdateUserName(UpdateUserNameRequestDto request, UserSessionModel user);
        Task<RefreshTokens> GenerateRefreshToken(string email);
        Task UpdateUserRefreshToken(string userEmail, RefreshTokens refreshToken);
    }
}
