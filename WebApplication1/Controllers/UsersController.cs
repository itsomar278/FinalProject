using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Response;
using WebApplication1.Services.Authentication;
using WebApplication1.UnitOfWorks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthinticateService _authinticateService;
        const int maxUsersPageSize = 7;
        public UsersController(IUnitOfWork unitOfWork, IAuthinticateService authinticateService)
        {
            _unitOfWork = unitOfWork;
            _authinticateService = authinticateService;
        }
        [HttpGet, Authorize]
        public ActionResult<Articles> GetUsers(string? searchQuery, int pageNumber = 1, int pageSize = 3)
        {
            if (pageSize > maxUsersPageSize)
            {
                pageSize = maxUsersPageSize;
            }
            var users = _unitOfWork.Users.GetUsers(searchQuery, pageNumber, pageSize);
            if (users.Count() == 0)
            {
                return NotFound("there is no users found !");
            }
            else
            {
                List<UsersResponse> usersResponses = new List<UsersResponse>();
                foreach (var user in users)
                {
                    UsersResponse userResponse = new UsersResponse
                    {
                        UserEmail = user.UserEmail,
                        UserName = user.UserName,
                    };
                    usersResponses.Add(userResponse);
                }
                return Ok(usersResponses);
            }
        }

        [HttpGet("{UserId}"), Authorize]
        public ActionResult GetUser([FromRoute(Name = "UserId")] int userId)
        {
            var user = _unitOfWork.Users.Get(userId);
            if (user == null)
            {
                return NotFound("cannot find the specified user");
            }
            else
            {
                UsersResponse userRespones = new UsersResponse
                {
                    UserEmail = user.UserEmail,
                    UserName = user.UserName,
                };
                return Ok(userRespones);
            }
        }
        [HttpGet("{UserId}/followers"), Authorize]
        public ActionResult GetFollowers([FromRoute(Name = "UserId")] int userId)
        {
            var user = _unitOfWork.Users.Get(userId);
            if (user == null)
            {
                return NotFound("cannot find the specified user");
            }
            var followersId = _unitOfWork.Follows.GetAllFollowersId(userId);
            if (followersId.Count() == 0)
            {
                return Ok(" 0 followers");
            }
            List<UsersResponse> usersResponses = new List<UsersResponse>();
            foreach (int id in followersId)
            {
                var follower = _unitOfWork.Users.Get(id);
                UsersResponse response = new UsersResponse
                {
                    UserName = follower.UserName,
                    UserEmail = follower.UserEmail,
                };
                usersResponses.Add(response);
            }
            return Ok(usersResponses);
        }
        [HttpGet("{UserId}/following"), Authorize]
        public ActionResult GetFollowing([FromRoute(Name = "UserId")] int userId)
        {
            var user = _unitOfWork.Users.Get(userId);
            if (user == null)
            {
                return NotFound("cannot find the specified user");
            }
            var followingId = _unitOfWork.Follows.GetAllFollowingId(userId);
            if (followingId.Count() == 0)
            {
                return Ok(" 0 followers");
            }
            List<UsersResponse> usersResponses = new List<UsersResponse>();
            foreach (int id in followingId)
            {
                var followed = _unitOfWork.Users.Get(id);
                UsersResponse response = new UsersResponse
                {
                    UserName = followed.UserName,
                    UserEmail = followed.UserEmail,
                };
                usersResponses.Add(response);
            }
            return Ok(usersResponses);
        }
        [HttpPost("{UserId}/following/{UserToFollowId}"), Authorize]
        public ActionResult follow([FromRoute(Name = "UserId")] int userId, [FromRoute(Name = "UserToFollowId")] int UserToFollowId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = _unitOfWork.Users.Find(u => u.UserEmail == userEmail).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            if (user.UserId != userId)
            {
                return Unauthorized();
            }
            var userToFollow = _unitOfWork.Users.Get(UserToFollowId);
            if(userToFollow == null)
            {
                return NotFound("the specified user doesn't exist");
            }
            var followCheck = _unitOfWork.Follows.Get(userId, UserToFollowId);
            if (followCheck != null)
            {
                return BadRequest("you already follow that user");
            }
            _unitOfWork.Follows.FollowUser(userId, UserToFollowId);
            _unitOfWork.complete();
            return Ok("user followed succesfully");
            
        }
        [HttpDelete("{UserId}/followers/{UserToUnfollowId}"), Authorize]
        public ActionResult Unfollow([FromRoute(Name = "UserId")] int userId , [FromRoute(Name = "UserToUnfollowId")]int UserToUnfollowId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = _unitOfWork.Users.Find(u => u.UserEmail == userEmail).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            if (user.UserId != userId)
            {
                return Unauthorized();
            }
            var follow = _unitOfWork.Follows.Get(userId, UserToUnfollowId);
            if (follow == null)
            {
                return BadRequest("you already not following that user");
            }
            _unitOfWork.Follows.Remove(follow);
            _unitOfWork.complete();
            return Ok("user unfollowed succesfully");
        }
        [HttpDelete("{UserId}/following/{UserToRemove}"), Authorize]
        public ActionResult RemoveFollower([FromRoute(Name = "UserId")] int userId, [FromRoute(Name = "UserToRemove")] int UserToRemove)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = _unitOfWork.Users.Find(u => u.UserEmail == userEmail).FirstOrDefault();
            if(user == null)
            {
                return NotFound();
            }
            if( user.UserId != userId)
            {
                return Unauthorized();
            }
            var follow = _unitOfWork.Follows.Get(UserToRemove, userId);
            if (follow == null)
            {
                return BadRequest("this user already doesn't follow you ");
            }
            _unitOfWork.Follows.Remove(follow); 
            _unitOfWork.complete();
            return Ok("user removed from followers successfully");
        }
       [HttpGet("{UserId}/favorite-Articles"), Authorize]
        public ActionResult GetFavoriteArticles([FromRoute(Name = "UserId")] int userId , int pageNumber = 1, int pageSize = 2)
        {
            var user = _unitOfWork.Users.Get(userId);
            if (user == null)
            {
                return NotFound("cannot find the specified user");
            }
            var favoriteArticles = _unitOfWork.Favorites.Find(f=>f.UserId == userId);
            if (favoriteArticles.Count() == 0)
            {
                return Ok("user has no favorite articles");
            }
            List<ArticleResponse> articleResponses= new List<ArticleResponse>();
            foreach(var favorite in favoriteArticles.Skip(pageSize * (pageNumber - 1)).Take(pageSize))
            {
                var article = _unitOfWork.Articles.Get(favorite.ArticleId);
                var author = _unitOfWork.Users.Get(article.UserId);
                ArticleResponse articleResponse = new ArticleResponse
                {
                    Title = article.Title,
                    Content = article.Content,
                    AuthorUserName = author.UserName
                };
                articleResponses.Add(articleResponse);
            }
            return Ok(articleResponses);
        }
    }
}
