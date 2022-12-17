using AutoMapper;
using Domain.Exceptions;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace Domain.Services
{
    public class UsersService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UsersService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper; 
        }

        public async Task<IEnumerable<UsersResponse>> GetUsers(string? searchQuery, int pageNumber = 1, int pageSize = 3)
        {
            var users = _unitOfWork.Users.GetUsers(searchQuery, pageNumber, pageSize);
            var usersResponses = _mapper.Map<List<UsersResponse>>(users);
            return await Task.FromResult(usersResponses);
        }
        public async Task<UsersResponse> GetUser(int userId)
        {
            if (!_unitOfWork.Users.DoesExist(u => u.UserId == userId))
            {
                throw new  RecordNotFoundException("cannot find the specified user");
            }
            var user = _unitOfWork.Users.Get(userId);
            var userResponse = _mapper.Map<UsersResponse>(user);
            return await Task.FromResult(userResponse);
        }
        public async Task<IEnumerable<UsersResponse>> GetFollowers(int userId)
        {
            if (!_unitOfWork.Users.DoesExist(u => u.UserId == userId))
            {
                throw new RecordNotFoundException("cannot find the specified user");
            }
            var followersId = _unitOfWork.Follows.GetAllFollowersId(userId);
            List<UsersResponse> usersResponses = new List<UsersResponse>();
            foreach (int id in followersId)
            {
                var follower = _unitOfWork.Users.Get(id);
                var response = _mapper.Map<UsersResponse>(follower);
                usersResponses.Add(response);
            }
            return await Task.FromResult(usersResponses);
        }
        public async Task<IEnumerable<UsersResponse>> GetFollowing(int userId)
        {
            if (!_unitOfWork.Users.DoesExist(u => u.UserId == userId))
            {
                throw new RecordNotFoundException("cannot find the specified user");
            }
            var followingId = _unitOfWork.Follows.GetAllFollowingId(userId);
            List<UsersResponse> usersResponses = new List<UsersResponse>();
            foreach (int id in followingId)
            {
                var followed = _unitOfWork.Users.Get(id);
                var response = _mapper.Map<UsersResponse>(followed);
                usersResponses.Add(response);
            }
            return await Task.FromResult(usersResponses);
        }
        public async Task<ActionResult> Follow(int userId, FollowRequest followRequest, UserSessionModel sessionUser)
        {
            if (!_unitOfWork.Users.DoesExist(u => u.UserId == userId))
            {
                throw new RecordNotFoundException("cannot find the specified user");
            }
            if (userId != sessionUser.UserId)
            {
                throw new UnauthorizedUserException("Unauthorized");
            }
            if (!_unitOfWork.Users.DoesExist(u => u.UserId == followRequest.UserToFollowId))
            {
                throw new RecordNotFoundException("cant find a user to follow with such id");
            }
            if (_unitOfWork.Follows.DoesExist(f => f.FollowerId == userId && f.FollowedId == followRequest.UserToFollowId))
            {
                throw new AlreadyExistingRecordException("you already follow this user");
            }
            var follow = _mapper.Map<Follow>((followRequest, userId));
            _unitOfWork.Follows.Add(follow);
            _unitOfWork.complete();
            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> Unfollow(int userId, int UserToUnfollowId, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }
            if (sessionUser.UserId != userId)
            {
                throw new UnauthorizedUserException("Unauthorized action");

            }
            if (!_unitOfWork.Follows.DoesExist(f => f.FollowerId == userId && f.FollowedId == UserToUnfollowId))
            {
                throw new RecordNotFoundException("you already not following that user");
            }
            var follow = _unitOfWork.Follows.Get((userId, UserToUnfollowId));
            _unitOfWork.Follows.Remove(follow);
            _unitOfWork.complete();
            return await Task.FromResult(new OkResult());   
        }
        public async Task<ActionResult> RemoveFollower(int userId , int userToRemoveId , UserSessionModel sessionUser)
        {
            if (sessionUser == null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }

            if (sessionUser.UserId != userId)
            {
                throw new UnauthorizedUserException("Unauthorized action");
            }

            if (_unitOfWork.Follows.DoesExist(f => f.FollowedId == userId && f.FollowerId == userToRemoveId))
            {
                throw new RecordNotFoundException("this user already doesn't follow you ");
            }

            var follow = _unitOfWork.Follows.Get((userToRemoveId, userId));
            _unitOfWork.Follows.Remove(follow);
            _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
        public async Task<IEnumerable<ArticleResponse>> GetFavoriteArticles(int userId, int pageNumber, int pageSize)
        {
            var user = _unitOfWork.Users.Get(userId);
            if (user == null)
            {
                throw new RecordNotFoundException("cannot find the specifed user ");
            }
            var favoriteArticles = _unitOfWork.Favorites.Find(f => f.UserId == userId);
            List<ArticleResponse> articleResponses = new List<ArticleResponse>();
            foreach (var favorite in favoriteArticles.Skip(pageSize * (pageNumber - 1)).Take(pageSize))
            {
                var article = _unitOfWork.Articles.Get(favorite.ArticleId);
                var author = _unitOfWork.Users.Get(article.UserId);
                var articleResponse = _mapper.Map<ArticleResponse>((article, user));
                articleResponses.Add(articleResponse);
            }
            return await Task.FromResult(articleResponses);
        }
        public async Task<ActionResult> AddToFavoriteArticles(int userId, AddToFavouritesRequest request , UserSessionModel sessionUser)
        {
            if (sessionUser == null)
            {
                throw new RecordNotFoundException("cannot find the specifed user ");
            }
            if (sessionUser.UserId != userId)
            {
                throw new UnauthorizedUserException("Unauthorized action");
            }
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == request.ArticleId))
            {
                throw new RecordNotFoundException("cannot find an article with such id");
            }
            if (_unitOfWork.Favorites.DoesExist(f => f.UserId == userId && f.ArticleId == request.ArticleId))
            {
                throw new AlreadyExistingRecordException("you already have this article in your favorites");
            }
            var favorite = _mapper.Map<Favorite>((request, userId));
            _unitOfWork.Favorites.Add(favorite);
            _unitOfWork.complete();
            return await Task.FromResult(new OkResult());
        }
        public async Task<ActionResult> RemoveFromFavourites(int userId, RemoveFromFavouritesRequest request, UserSessionModel sessionUser)
        {
            if (sessionUser == null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }
            if (sessionUser.UserId != userId)
            {
                throw new UnauthorizedUserException("Unauthorized action");
            }
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == request.ArticleId))
            {
                throw new RecordNotFoundException("cannot find an article with such id");
            }
            if (!_unitOfWork.Favorites.DoesExist(f => f.UserId == userId && f.ArticleId == request.ArticleId))
            {
                throw new RecordNotFoundException("there is no article with such id in your favorites");
            }
            var favorite = _unitOfWork.Favorites.Get((request.ArticleId, userId));
            _unitOfWork.Favorites.Remove(favorite);
            _unitOfWork.complete();
            return await Task.FromResult(new OkResult());
        }
    }
}
