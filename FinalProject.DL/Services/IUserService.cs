using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UsersResponse>> GetUsers(string? searchQuery, int pageNumber = 1, int pageSize = 3);
        Task<UsersResponse> GetUser(int userId);
        Task<IEnumerable<UsersResponse>> GetFollowers(int userId);
        Task<IEnumerable<UsersResponse>> GetFollowing(int userId);
        Task<ActionResult> Follow(int userId, FollowRequest followRequest, UserSessionModel sessionUser);
        Task<ActionResult> Unfollow(int userId, int UserToUnfollowId, UserSessionModel sessionUser);
        Task<ActionResult> RemoveFollower(int userId, int UserToRemoveId, UserSessionModel sessionUser);
        Task<IEnumerable<ArticleResponse>> GetFavoriteArticles(int userId, int pageNumber, int pageSize);
        Task<ActionResult> AddToFavoriteArticles(int userId, AddToFavouritesRequest request , UserSessionModel sessionUser);
        Task<ActionResult> RemoveFromFavourites(int userId, RemoveFromFavouritesRequest request, UserSessionModel sessionUser);

    }
}
