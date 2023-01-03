using AutoMapper;
using Domain.Models.DTO_s.RequestDto_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Services.AuthService;
using WebApplication1.Services.Session;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authinticateService;
        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public AuthController( IAuthenticationService authinticateService, ISessionDataManagment sessionDataManagment , IHttpContextAccessor httpContextAccessor
            , IMapper mapper )
        {
            _authinticateService = authinticateService;
            _sessionDataManagment = sessionDataManagment;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Rigester(UserRegisterRequest request)
        {
            var requestDto = _mapper.Map<UserRegisterRequestDto>(request);
            await _authinticateService.Register(requestDto);
            return Ok("user registerd successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginRequest request)
        {
            var requestDto = _mapper.Map<UserLoginRequestDto>(request);
            var token = await _authinticateService.login(requestDto);

            var refreshToken = await _authinticateService.GenerateRefreshToken(request.UserEmail);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            await _authinticateService.UpdateUserRefreshToken(request.UserEmail, refreshToken);

            await _sessionDataManagment.StoreUserInSession(request.UserEmail);

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
            var newRefreshToken = await _authinticateService.GenerateRefreshToken(sessionUser.UserEmail);
            await _authinticateService.UpdateUserRefreshToken(sessionUser.UserEmail, newRefreshToken);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            await _sessionDataManagment.StoreUserInSession(sessionUser.UserEmail);
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
            var requestDto = _mapper.Map<UserPasswordUpdateRequestDto>(request);
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _authinticateService.UpdatePassword(requestDto, sessionUser);
            return Ok("Password updated successfully");
        }

        [HttpPost("update-userName"), Authorize]
        public async Task<ActionResult> UpdateUserName(UpdateUserNameRequest request)
        {
            var requestDto = _mapper.Map<UpdateUserNameRequestDto>(request);
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _authinticateService.UpdateUserName(requestDto, sessionUser);
            return Ok("User Name updated successfully");
        }
        
    }
}