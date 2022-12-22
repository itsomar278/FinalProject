using AutoMapper;
using Domain.Exceptions;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;

namespace WebApplication1.Services.Authentication
{
    public class AuthenticationService : IAuthinticateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim(ClaimTypes.Email , user.UserEmail)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("myfinalprojectforts123"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        public async Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return await Task.FromResult(computedHash.SequenceEqual(passwordHash));
            }
        }
        public async Task<RefreshTokens> GenerateRefreshToken(string email)
        {
            var user = await _unitOfWork.Users.FindByEmailAsync(email);

            var refreshToken = new RefreshTokens
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                UserId = user.UserId
            };

            return await Task.FromResult(refreshToken);
        }
        public async Task<ActionResult> Register(UserRegisterRequest request)
        {
            if ((await _unitOfWork.Users.DoesExistAsync(u => u.UserEmail == request.UserEmail)) ||
                (await _unitOfWork.Users.DoesExistAsync(u => u.UserName == request.UserName)))
            {
                throw new AlreadyExistingRecordException("Email or User Name has been used before"); // some sort of problem is here 
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = _mapper.Map<Users>((request, passwordHash, passwordSalt));
            _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<string> login(UserLoginRequest request)
        {
            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserEmail == request.UserEmail))
                throw new RecordNotFoundException("There is no user with such email address");
            

            var userResult = await _unitOfWork.Users.FindByEmailAsync(request.UserEmail);
            if (!(await VerifyPasswordHash(request.Password, userResult.PasswordHash, userResult.PasswordSalt)))
                throw new UnauthorizedUserException("wrong passowrd !!!!!");    

            var refreshTokenResult = await _unitOfWork.RefreshTokens.FindAsync(t => t.UserId == userResult.UserId);

            if (refreshTokenResult.Count() != 0)
            {
                _unitOfWork.RefreshTokens.RemoveRange(refreshTokenResult);
                await _unitOfWork.complete();
            }

            var token = CreateToken(userResult);

            return await Task.FromResult(token);
        }
        public async Task<string> RefreshToken(UserSessionModel sessionUser, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken) || sessionUser is null)
                throw new UnauthorizedUserException("you need to re-login");

            var myRefreshToken = await _unitOfWork.RefreshTokens.GetAsync(sessionUser.RefreshTokenId);
            if (myRefreshToken.Token != refreshToken || myRefreshToken.Expires < DateTime.Now)
                throw new UnauthorizedUserException("you need to re-login");

            var user = await _unitOfWork.Users.GetAsync(sessionUser.UserId);
            string token = CreateToken(user);

            return await Task.FromResult(token);
        }
        public async Task<ActionResult> logout(UserSessionModel user)
        {
            if (user == null)
                throw new UnauthorizedUserException("you are not logged in in the first place");

            if (await _unitOfWork.RefreshTokens.DoesExistAsync(rt => rt.UserId == user.UserId))
                DeleteUserRefreshToken(user.UserEmail);

            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> UpdatePassword(UserPasswordUpdateRequest request, UserSessionModel sessionUser)
        {
            var user = await _unitOfWork.Users.FindByEmailAsync(request.UserEmail);
            if (user == null)
                throw new RecordNotFoundException("There is no user with such email address");

            if (sessionUser == null)
                throw new UnauthorizedUserException("you need to re-login");

            if (sessionUser.UserId != user.UserId)
                throw new RecordNotFoundException("you cant change other user passwword");

            if (!await VerifyPasswordHash(request.OldPassword, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedUserException("Incorrect old password");

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> UpdateUserName(UpdateUserNameRequest request, UserSessionModel sessionUser)
        {
            var user = await _unitOfWork.Users.FindByEmailAsync(request.UserEmail);
            if (user == null)
                throw new RecordNotFoundException("There is no user with such email address");


            if (sessionUser == null)
                throw new UnauthorizedUserException("you need to re-login");


            if (user.UserId != sessionUser.UserId)
                throw new UnauthorizedUserException("you cant change someone else password");

            if (!await VerifyPasswordHash(request.userPassword, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedUserException("Incorrect password");

            user.UserName = request.NewUserName;
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task UpdateUserRefreshToken(string userEmail, RefreshTokens refreshToken)
        {
            var userResult = await _unitOfWork.Users.FindByEmailAsync(userEmail);
            if (await _unitOfWork.RefreshTokens.DoesExistAsync(rt => rt.UserId == userResult.UserId))
            {
                var refreshTokenToDelete = await _unitOfWork.RefreshTokens.FindAsync(rt => rt.UserId == userResult.UserId);
                _unitOfWork.RefreshTokens.RemoveRange(refreshTokenToDelete);
                await _unitOfWork.complete();
            }
            _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.complete();
            _unitOfWork.Users.UpdateUserRefreshToken(userResult.UserId, refreshToken.TokenId);
            await _unitOfWork.complete();

        }
        public async Task DeleteUserRefreshToken(string userEmail)
        {
            var userResult = await _unitOfWork.Users.FindByEmailAsync(userEmail);

            if (await _unitOfWork.RefreshTokens.DoesExistAsync(rt => rt.UserId == userResult.UserId))
            {
                var refreshTokenToDelete = await _unitOfWork.RefreshTokens.FindAsync(rt => rt.UserId == userResult.UserId);
                _unitOfWork.RefreshTokens.RemoveRange(refreshTokenToDelete);
                await _unitOfWork.complete();
            }

            _unitOfWork.Users.UpdateUserRefreshToken(userResult.UserId, 0);
            await _unitOfWork.complete();
        }

    }
}


