﻿using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;

namespace WebApplication1.Services.Authentication
{
    public interface IAuthinticateService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Users user);
        Task<string> RefreshToken(UserSessionModel sessionUser, string refreshToken);
        Task<string> login(UserLoginRequest request);
        Task<ActionResult> logout(UserSessionModel user);
        Task<ActionResult> Register(UserRegisterRequest request);
        Task<ActionResult> UpdatePassword(UserPasswordUpdateRequest request , UserSessionModel user);
        Task<ActionResult> UpdateUserName(UpdateUserNameRequest request, UserSessionModel user);
        RefreshTokens GenerateRefreshToken(string email);
        void UpdateUserRefreshToken(string userEmail, RefreshTokens refreshToken);
    }
}
