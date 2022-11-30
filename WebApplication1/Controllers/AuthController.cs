using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApplication1.Models.Entites;
using WebApplication1.Services.Authentication;
using WebApplication1.UnitOfWorks ;

namespace WebApplication1.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthinticateService _authinticateService;

        public AuthController(IUnitOfWork unitOfWork , IAuthinticateService authinticateService)
		{
            _unitOfWork = unitOfWork;
            _authinticateService = authinticateService;

        }
		[HttpPost("register")]
		public async Task<ActionResult<Users>> Rigester(UserRegisterRequest request)
		{
			var usedUserName = _unitOfWork.Users.Find(u => u.UserName == request.UserName).ToList();
			var UsedEmail = _unitOfWork.Users.Find(u => u.UserEmail == request.UserEmail).ToList();
            if (usedUserName.Count!=0|| UsedEmail.Count!=0)
			{ 
                return BadRequest("Email or User Name has been used before");
			}
            _authinticateService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			var user = new Users
			{
				UserEmail= request.UserEmail,
				UserName = request.UserName,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};
			_unitOfWork.Users.Add(user);
            _unitOfWork.complete();
            return Ok(user);
		}
		[HttpPost("login")]
        public async Task<ActionResult<Users>> Login(UserLoginRequest request)
		{
			var userResult = _unitOfWork.Users.FindByEmail(request.UserEmail);
			var refreshTokenResult = _unitOfWork.RefreshTokens.Find(t => t.UserId == userResult.UserId);
			if (refreshTokenResult.Count()!=0 ) 
			{
				_unitOfWork.RefreshTokens.RemoveRange(refreshTokenResult);
				_unitOfWork.complete();
			}
		    if (userResult != null && _authinticateService.VerifyPasswordHash(request.Password, userResult.PasswordHash, userResult.PasswordSalt))
			{
			    string token = _authinticateService.CreateToken(userResult);
				RefreshTokens refreshToken = _authinticateService.GenerateRefreshToken(userResult.UserId);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = refreshToken.Expires
                };
                Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
                _unitOfWork.RefreshTokens.Add(refreshToken);
                _unitOfWork.complete();
                _unitOfWork.Users.UpdateUserRefreshToken(userResult, refreshToken.TokenId);
				_unitOfWork.complete();
                return Ok(token);
			}
			else
			{
                return BadRequest("either email is wrong or password");
            }
        }
		[HttpPost("refresh"),Authorize]
        public async Task<ActionResult<string>> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var user = _unitOfWork.Users.Find(u => u.UserEmail == userEmail).FirstOrDefault();
			if (user!=null)
			{
		       	var myRefreshToken = _unitOfWork.RefreshTokens.Get(user.RefreshTokenId.Value);
				if(myRefreshToken.Token!=refreshToken|| myRefreshToken.Expires < DateTime.Now)
				{
					return Unauthorized("invalid refresh token");
				}
				else
				{
					string token = _authinticateService.CreateToken(user);
                    var newRefreshToken = _authinticateService.GenerateRefreshToken(user.UserId);
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = newRefreshToken.Expires
                    };
                    Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
					_unitOfWork.RefreshTokens.Add(newRefreshToken); ;
					_unitOfWork.complete();
					_unitOfWork.Users.UpdateUserRefreshToken(user, newRefreshToken.TokenId);
                    _unitOfWork.complete();
					return Ok("refreshed");

                }


            }
			else
			{
				return BadRequest();
			}
				
			



        }
        [HttpPost("logout"), Authorize]
        public async Task<ActionResult<string>> Logout()
		{
			var userToLogout = _unitOfWork.Users.FindByEmail(User.FindFirstValue(ClaimTypes.Email));
			var tokensToDelete = _unitOfWork.RefreshTokens.Find(rt => rt.UserId == userToLogout.UserId);
			_unitOfWork.RefreshTokens.RemoveRange(tokensToDelete);
            _unitOfWork.complete();
			return Ok("user loged out succesfully");
		}
    }
}