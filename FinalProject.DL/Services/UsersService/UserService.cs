using AutoMapper;
using Domain.Exceptions;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.Requests;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;


namespace Domain.Services.UsersService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsers(UserSearchRequestDto userSearchRequest )
        {
            var users = await _unitOfWork.Users.GetUsersAsync(userSearchRequest.searchQuery, userSearchRequest.pageNumber, userSearchRequest.pageSize);
            var usersResponses = _mapper.Map<List<UserResponseDto>>(users);

            return await Task.FromResult(usersResponses);
        }
        public async Task<UserResponseDto> GetUser(int userId)
        {
            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserId == userId))
                throw new RecordNotFoundException("cannot find the specified user");

            var user = await _unitOfWork.Users.GetAsync(userId);
            var userResponse = _mapper.Map<UserResponseDto>(user);

            return await Task.FromResult(userResponse);
        }
        public async Task<IEnumerable<UserResponseDto>> GetFollowers(int userId)
        {
            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserId == userId))
                throw new RecordNotFoundException("Cannot find the specified user");

            var followersId = await _unitOfWork.Follows.GetAllFollowersIdAsync(userId);
            List<UserResponseDto> usersResponses = new List<UserResponseDto>();


            foreach (int id in followersId)
            {
                var follower = await _unitOfWork.Users.GetAsync(id);
                var response = _mapper.Map<UserResponseDto>(follower);
                usersResponses.Add(response);
            }

            return await Task.FromResult(usersResponses);
        }
        public async Task<IEnumerable<UserResponseDto>> GetFollowing(int userId)
        {
            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserId == userId))
                throw new RecordNotFoundException("cannot find the specified user");

            var followingId = await _unitOfWork.Follows.GetAllFollowingId(userId);
            List<UserResponseDto> usersResponses = new List<UserResponseDto>();

            foreach (int id in followingId)
            {
                var followed = await _unitOfWork.Users.GetAsync(id);
                var response = _mapper.Map<UserResponseDto>(followed);
                usersResponses.Add(response);
            }

            return await Task.FromResult(usersResponses);
        }
        public async Task<ActionResult> Follow(int userId, FollowRequest followRequest, UserSessionModel sessionUser)
        {
            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserId == userId))
                throw new RecordNotFoundException("cannot find the specified user");

            if (userId != sessionUser.UserId)
                throw new UnauthorizedUserException("Unauthorized");

            if (!await _unitOfWork.Users.DoesExistAsync(u => u.UserId == followRequest.UserToFollowId))
                throw new RecordNotFoundException("cant find a user to follow with such id");

            if (await _unitOfWork.Follows.DoesExistAsync(f => f.FollowerId == userId && f.FollowedId == followRequest.UserToFollowId))
                throw new AlreadyExistingRecordException("you already follow this user");

            var follow = _mapper.Map<Follow>((followRequest, userId));
            _unitOfWork.Follows.AddAsync(follow);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> Unfollow(int userId, int UserToUnfollowId, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
                throw new UnauthorizedUserException("you need to re-login");

            if (sessionUser.UserId != userId)
                throw new UnauthorizedUserException("Unauthorized action");

            if (!await _unitOfWork.Follows.DoesExistAsync(f => f.FollowerId == userId && f.FollowedId == UserToUnfollowId))
                throw new RecordNotFoundException("you already not following that user");

            var follow = await _unitOfWork.Follows.GetAsync((userId, UserToUnfollowId));
            _unitOfWork.Follows.Remove(follow);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> RemoveFollower(int userId, int userToRemoveId, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
                throw new UnauthorizedUserException("you need to re-login");

            if (sessionUser.UserId != userId)
                throw new UnauthorizedUserException("Unauthorized action");

            if (!await _unitOfWork.Follows.DoesExistAsync(f => f.FollowedId == userId && f.FollowerId == userToRemoveId))
                throw new RecordNotFoundException("this user already doesn't follow you ");

            var follow = await _unitOfWork.Follows.GetAsync((userToRemoveId, userId));
            _unitOfWork.Follows.Remove(follow);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<IEnumerable<ArticleResponse>> GetFavoriteArticles(int userId, PagingRequest pagingRequest)
        {
            var user = await _unitOfWork.Users.GetAsync(userId);

            if (user == null)
                throw new RecordNotFoundException("cannot find the specifed user ");

            var favoriteArticles = await _unitOfWork.Favorites.FindAsync(f => f.UserId == userId);
            List<ArticleResponse> articleResponses = new List<ArticleResponse>();

            foreach (var favorite in favoriteArticles.Skip(pagingRequest.pageSize * (pagingRequest.pageNumber - 1)).Take(pagingRequest.pageSize))
            {
                var article = await _unitOfWork.Articles.GetAsync(favorite.ArticleId);
                var author = await _unitOfWork.Users.GetAsync(article.UserId);
                var articleResponse = _mapper.Map<ArticleResponse>((article, user));
                articleResponses.Add(articleResponse);
            }

            return await Task.FromResult(articleResponses);
        }
        public async Task<ActionResult> AddToFavoriteArticles(int userId, AddToFavouritesRequest request, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
                throw new RecordNotFoundException("cannot find the specifed user ");

            if (sessionUser.UserId != userId)
                throw new UnauthorizedUserException("Unauthorized action");

            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == request.ArticleId))
                throw new RecordNotFoundException("cannot find an article with such id");

            if (await _unitOfWork.Favorites.DoesExistAsync(f => f.UserId == userId && f.ArticleId == request.ArticleId))
                throw new AlreadyExistingRecordException("you already have this article in your favorites");

            var favorite = _mapper.Map<Favorite>((request, userId));
            _unitOfWork.Favorites.AddAsync(favorite);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> RemoveFromFavourites(int userId, RemoveFromFavouritesRequest request, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
                throw new UnauthorizedUserException("you need to re-login");

            if (sessionUser.UserId != userId)
                throw new UnauthorizedUserException("Unauthorized action");

            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == request.ArticleId))
                throw new RecordNotFoundException("cannot find an article with such id");

            if (!await _unitOfWork.Favorites.DoesExistAsync(f => f.UserId == userId && f.ArticleId == request.ArticleId))
                throw new RecordNotFoundException("there is no article with such id in your favorites");

            var favorite = await _unitOfWork.Favorites.GetAsync((request.ArticleId, userId));
            _unitOfWork.Favorites.Remove(favorite);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
    }
}
