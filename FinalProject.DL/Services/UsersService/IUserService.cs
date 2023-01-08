using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Services.SessionService;
using Microsoft.AspNetCore.Mvc;


namespace Domain.Services.UsersService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsers(UserSearchRequestDto userSearchRequest);
        Task<UserResponseDto> GetUser(int userId);
        Task<IEnumerable<UserResponseDto>> GetFollowers(int userId);
        Task<IEnumerable<UserResponseDto>> GetFollowing(int userId);
        Task<ActionResult> Follow(int userId, FollowRequestDto followRequest, UserSessionModel sessionUser);
        Task<ActionResult> Unfollow(int userId, UnfollowRequestDto unfollowRequest, UserSessionModel sessionUser);
        Task<ActionResult> RemoveFollower(int userId, RemoveFollowerRequestDto removeFollowerRequest, UserSessionModel sessionUser);
        Task<IEnumerable<ArticleResponseDto>> GetFavoriteArticles(int userId, PagingRequestDto pagingRequest);
        Task<ActionResult> AddToFavoriteArticles(int userId, AddToFavouritesRequestDto request, UserSessionModel sessionUser);
        Task<ActionResult> RemoveFromFavourites(int userId, RemoveFromFavouritesRequestDto request, UserSessionModel sessionUser);

    }
}
