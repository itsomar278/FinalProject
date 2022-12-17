using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Services.Authentication;
using WebApplication1.Services.SessionManagment;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthinticateService _authinticateService;
        private readonly ISessionDataManagment _sessionDataManagment;
        public AuthController( IAuthinticateService authinticateService, ISessionDataManagment sessionDataManagment)
        {
            _authinticateService = authinticateService;
            _sessionDataManagment = sessionDataManagment;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Users>> Rigester(UserRegisterRequest request)
        {
            await _authinticateService.Register(request);
            return Ok("user registerd successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginRequest request)
        {
            var token = await _authinticateService.login(request);

            var refreshToken = _authinticateService.GenerateRefreshToken(request.UserEmail);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            _authinticateService.UpdateUserRefreshToken(request.UserEmail, refreshToken);

            _sessionDataManagment.StoreUserInSession(request.UserEmail);

            return Ok(token);
        }

        [HttpPost("refresh"), Authorize]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session" || cookie == "refreshToken")
                    Response.Cookies.Delete(cookie);
            }
            string token = await _authinticateService.RefreshToken(sessionUser, refreshToken);
            var newRefreshToken = _authinticateService.GenerateRefreshToken(sessionUser.UserEmail);
            _authinticateService.UpdateUserRefreshToken(sessionUser.UserEmail, newRefreshToken);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            _sessionDataManagment.StoreUserInSession(sessionUser.UserEmail);
            return Ok(token);
        }

        [HttpPost("logout"), Authorize]
        public async Task<ActionResult> Logout()
        {
            var userToLogout = _sessionDataManagment.GetUserFromSession();
            await _authinticateService.logout(userToLogout);
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session" || cookie == "refreshToken")
                    Response.Cookies.Delete(cookie);
            }
            return Ok("user loged out succesfully");
        }

        [HttpPost("update-password"), Authorize]
        public async Task<ActionResult> UpdatePassword(UserPasswordUpdateRequest request)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _authinticateService.UpdatePassword(request, sessionUser);
            return Ok("Password updated successfully");
        }

        [HttpPost("update-userName"), Authorize]
        public async Task<ActionResult> UpdateUserName(UpdateUserNameRequest request)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _authinticateService.UpdateUserName(request, sessionUser);
            return Ok("User Name updated successfully");
        }
        private void DeleteSession()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session")
                    Response.Cookies.Delete(cookie);
            }
        }
    }
}