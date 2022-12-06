using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Services.Authentication;
using WebApplication1.Services.SessionManagment;
using WebApplication1.UnitOfWorks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthinticateService _authinticateService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataManagment _sessionDataManagment;
        public AuthController(IUnitOfWork unitOfWork, IAuthinticateService authinticateService, IHttpContextAccessor httpContextAccessor,
            ISessionDataManagment sessionDataManagment)
        {
            _unitOfWork = unitOfWork;
            _authinticateService = authinticateService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataManagment = sessionDataManagment;

        }
        [HttpPost("register")]
        public ActionResult<Users> Rigester(UserRegisterRequest request)
        {
            if (_unitOfWork.Users.DoesExist(u => u.UserEmail == request.UserEmail) ||
                _unitOfWork.Users.DoesExist(u => u.UserName == request.UserName))
            {
                return BadRequest("Email or User Name has been used before");
            }
            _authinticateService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new Users
            {
                UserEmail = request.UserEmail,
                UserName = request.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _unitOfWork.Users.Add(user);
            _unitOfWork.complete();
            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<Users> Login(UserLoginRequest request)
        {
            if (!_unitOfWork.Users.DoesExist(u => u.UserEmail == request.UserEmail))
            {
                return NotFound("There is no user with such email address");
            }
            var userResult = _unitOfWork.Users.FindByEmail(request.UserEmail);
            if (!_authinticateService.VerifyPasswordHash(request.Password, userResult.PasswordHash, userResult.PasswordSalt))
            {
                return BadRequest("wrong passowrd !!!!!");
            }
            var refreshTokenResult = _unitOfWork.RefreshTokens.Find(t => t.UserId == userResult.UserId);
            if (refreshTokenResult.Count() != 0)
            {
                _unitOfWork.RefreshTokens.RemoveRange(refreshTokenResult);
                _unitOfWork.complete();
            }
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
            _unitOfWork.Users.UpdateUserRefreshToken(userResult.UserId, refreshToken.TokenId);
            _unitOfWork.complete();
            _sessionDataManagment.StoreUserInSession(userResult);
            return Ok(token);
        }

        [HttpPost("refresh"), Authorize]
        public ActionResult<string> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie == ".AspNetCore.Session")
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }
                return Unauthorized("you need to re-login");
            }
            var user = _sessionDataManagment.GetUserFromSession();
            if (user == null)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie == ".AspNetCore.Session")
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }
                return Unauthorized("you need to re-login");
            }
            var myRefreshToken = _unitOfWork.RefreshTokens.Get((user.RefreshTokenId.Value));
            if (myRefreshToken.Token != refreshToken || myRefreshToken.Expires < DateTime.Now)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie == ".AspNetCore.Session")
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }
                return Unauthorized("you need to re-login");
            }
            _unitOfWork.RefreshTokens.Remove(myRefreshToken);
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
            _unitOfWork.Users.UpdateUserRefreshToken(user.UserId, newRefreshToken.TokenId);
            _unitOfWork.complete();
            _sessionDataManagment.StoreUserInSession(user);
            return Ok(token);
        }
        [HttpPost("logout"), Authorize]
        public ActionResult<string> Logout()
        {
            var userToLogout = _sessionDataManagment.GetUserFromSession();
            if (userToLogout == null)
            {
                return Unauthorized(" you are already not logged in ");
            }
            if (_unitOfWork.RefreshTokens.DoesExist(rt => rt.UserId == userToLogout.UserId))
            {
                var tokensToDelete = _unitOfWork.RefreshTokens.Find(rt => rt.UserId == userToLogout.UserId);
                _unitOfWork.RefreshTokens.RemoveRange(tokensToDelete);
                _unitOfWork.complete();
            }
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session" || cookie == "refreshToken")
                    Response.Cookies.Delete(cookie);
            }
            _unitOfWork.Users.UpdateUserRefreshToken(userToLogout.UserId, 0);
            _unitOfWork.complete();
            return Ok("user loged out succesfully");
        }
        [HttpPost("update-password")]
        public ActionResult<string> UpdatePassword(UserPasswordUpdateRequest request)
        {
            var user = _unitOfWork.Users.FindByEmail(request.UserEmail);
            if (user == null)
            {
                return NotFound("There is no user with such email address");
            }
            if (!_authinticateService.VerifyPasswordHash(request.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Incorrect old password");
            }
            _authinticateService.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _unitOfWork.complete();
            return Ok("Password updated successfully");
        }
        [HttpPost("update-userName")]
        public ActionResult<string> UpdateUserName(UpdateUserNameRequest request)
        {
            var user = _unitOfWork.Users.FindByEmail(request.UserEmail);
            if (user == null)
            {
                return NotFound("There is no user with such email address");
            }
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            if(user.UserId != sessionUser.UserId)
            {
                return Unauthorized();
            }
            if (!_authinticateService.VerifyPasswordHash(request.userPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Incorrect password");
            }
            user.UserName= request.NewUserName;
            _unitOfWork.complete();
            return Ok("User Name updated successfully");
        }

    }
}