using AutoMapper;
using Domain.Models.Requests;
using Domain.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Services.AuthService;
using WebApplication1.Services.Session;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly IUserService _userService;
        const int maxUsersPageSize = 7;
        public UsersController(ISessionDataManagment sessionDataManagment, IUserService userService)
        {
            _sessionDataManagment = sessionDataManagment;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<UsersResponse>> GetUsers([FromQuery] UserSearchRequest userSearchRequest) // create request for these params
        {
            if (userSearchRequest.pageSize > maxUsersPageSize)
                userSearchRequest.pageSize = maxUsersPageSize;

            var users = await _userService.GetUsers(userSearchRequest);

            if (users.Count() == 0)
                return NotFound("there is no users registered yet !");

            return Ok(users);
        }

        [HttpGet("{UserId}"), Authorize]
        public async Task<ActionResult<UsersResponse>> GetUser([FromRoute(Name = "UserId")] int userId)
        {
            var userResponse = await _userService.GetUser(userId);
            return Ok(userResponse);
        }

        [HttpGet("{UserId}/followers"), Authorize]
        public async Task<ActionResult<UsersResponse>> GetFollowers([FromRoute(Name = "UserId")] int userId)
        {
            var userResponses = await _userService.GetFollowers(userId);

            if (userResponses.Count() == 0)
                return Ok("user have 0 followers");

            return Ok(userResponses);
        }

        [HttpGet("{UserId}/following"), Authorize]
        public async Task<ActionResult<UsersResponse>> GetFollowing([FromRoute(Name = "UserId")] int userId)
        {
            var userResponses = await _userService.GetFollowing(userId);

            if (userResponses.Count() == 0)
                return Ok("user doesn't follow any other user ");

            return Ok(userResponses);
        }

        [HttpPost("{UserId}/following"), Authorize]
        public async Task<ActionResult> Follow([FromRoute(Name = "UserId")] int userId, FollowRequest followRequest)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _userService.Follow(userId, followRequest, sessionUser);

            return Ok("user followed succesfully");
        }

        [HttpDelete("{UserId}/followers/{UserToUnfollowId}"), Authorize]
        public async Task<ActionResult> Unfollow([FromRoute(Name = "UserId")] int userId, [FromRoute(Name = "UserToUnfollowId")] int UserToUnfollowId)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _userService.Unfollow(userId, UserToUnfollowId, sessionUser);

            return Ok("user unfollowed succesfully");
        }

        [HttpDelete("{UserId}/following/{UserToRemove}"), Authorize]
        public async Task<ActionResult> RemoveFollower([FromRoute(Name = "UserId")] int userId, [FromRoute(Name = "UserToRemove")] int userToRemove)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _userService.RemoveFollower(userId, userToRemove, sessionUser);

            return Ok("user removed from followers successfully");
        }

        [HttpGet("{UserId}/favorite-Articles"), Authorize]
        public async Task<ActionResult<ArticleResponse>> GetFavoriteArticles([FromRoute(Name = "UserId")] int userId,[FromQuery] PagingRequest pagingRequest) 
        {
            var favoriteArticles = await _userService.GetFavoriteArticles(userId, pagingRequest);
            return Ok(favoriteArticles);
        }

        [HttpPost("{UserId}/favorite-Articles"), Authorize]
        public async Task<ActionResult> AddToFavoriteArticles([FromRoute(Name = "UserId")] int userId, AddToFavouritesRequest request)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _userService.AddToFavoriteArticles(userId, request, sessionUser);

            return Ok("Article successfully added to favorites");
        }

        [HttpDelete("{UserId}/favorite-Articles"), Authorize]
        public async Task<ActionResult> RemoveFromFavourites([FromRoute(Name = "UserId")] int userId, RemoveFromFavouritesRequest request)
        {
            var sessionUser = _sessionDataManagment.GetUserFromSession();
            await _userService.RemoveFromFavourites(userId, request, sessionUser);

            return Ok("Article successfully removed from yur favorites");
        }

    }
}
