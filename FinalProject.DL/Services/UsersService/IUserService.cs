using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace Domain.Services.UsersService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsers(UserSearchRequestDto userSearchRequest);
        Task<UserResponseDto> GetUser(int userId);
        Task<IEnumerable<UserResponseDto>> GetFollowers(int userId);
        Task<IEnumerable<UserResponseDto>> GetFollowing(int userId);
        Task<ActionResult> Follow(int userId, FollowRequest followRequest, UserSessionModel sessionUser);
        Task<ActionResult> Unfollow(int userId, int UserToUnfollowId, UserSessionModel sessionUser);
        Task<ActionResult> RemoveFollower(int userId, int UserToRemoveId, UserSessionModel sessionUser);
        Task<IEnumerable<ArticleResponse>> GetFavoriteArticles(int userId, PagingRequest pagingRequest);
        Task<ActionResult> AddToFavoriteArticles(int userId, AddToFavouritesRequest request, UserSessionModel sessionUser);
        Task<ActionResult> RemoveFromFavourites(int userId, RemoveFromFavouritesRequest request, UserSessionModel sessionUser);

    }
}
