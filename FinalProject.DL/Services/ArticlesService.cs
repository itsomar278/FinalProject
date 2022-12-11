using AutoMapper;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace FinalProject.DL.Services
{
    public class ArticlesService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ArticlesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ActionResult> ArticlePartialUpdate(int articleId, JsonPatchDocument<ArticlePostRequest> patchDocument , UserSessionModel user)
        {
            if ( user is null )
            {
                throw new UnauthorizedUserException("you need to relogin");
            }

            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("cannot find the specifed article");
            }

            var articleToUpdate = _unitOfWork.Articles.Get(articleId);
            if (user.UserId != articleToUpdate.UserId)
            {
                throw new UnauthorizedUserException("you cant upadte other users articles");
            }

            var articlePostRequest = _mapper.Map<ArticlePostRequest>(articleToUpdate);
            patchDocument.ApplyTo(articlePostRequest);
            _mapper.Map<ArticlePostRequest, Articles>(articlePostRequest, articleToUpdate);
            _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ActionResult> DeleteArticle(int articleId, UserSessionModel user)
        {
            if (user == null)
            {
                throw new UnauthorizedUserException("you need to relogin");
            }

            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("cannot find the specifed article");
            }

            var article = _unitOfWork.Articles.Get(articleId);
            if (article.UserId != user.UserId)
            {
                throw new UnauthorizedUserException("you cant delete this article ");
            }

           var related = _unitOfWork.Favorites.Find(f => f.ArticleId == articleId); // to avoid cycles and multi cascade pathes 
            _unitOfWork.Favorites.RemoveRange(related);
            _unitOfWork.complete();

            _unitOfWork.Articles.Remove(article);
            _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user , CommentDeleteRequest request)
        {
            if(user is null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }

            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("there is no article with the specified id");
            }

            if (!_unitOfWork.Comments.DoesExist(c => c.CommentId == request.commentId))
            {
                throw new  RecordNotFoundException("there is no comment with the specified id");
            }

            var comment = _unitOfWork.Comments.Get(request.commentId);
            if (user.UserId != comment.UserId)
            {
                throw new UnauthorizedUserException("you cant delete another user comment");
            }
  
            _unitOfWork.Comments.Remove(comment);
            _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ArticleResponse> GetArticle(int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("cannot find the specifed article");
            }

            var article = _unitOfWork.Articles.Get(articleId);
            var user = _unitOfWork.Users.Get(article.UserId);
            var response = _mapper.Map<ArticleResponse>((article, user));

            return await Task.FromResult(response);
        }


        public async Task<IEnumerable<ArticleResponse>> GetArticles(string? title, string? searchQuery, int pageNumber, int pageSize)
        {
            var articles = _unitOfWork.Articles.GetArticles(title, searchQuery, pageNumber, pageSize);

            if (articles.Count() == 0)
            {
                return Enumerable.Empty<ArticleResponse>();
            }

            List<ArticleResponse> articleResponses = new List<ArticleResponse>();
            foreach (var article in articles)
            {
                var user = _unitOfWork.Users.Get(article.UserId);
                var response = _mapper.Map<ArticleResponse>((article, user));
                articleResponses.Add(response);
            }

            return await Task.FromResult(articleResponses);
        }

        public async Task<CommentResponse> GetCommentOnArticle(int articleId, int commentId)
        {
            if (!_unitOfWork.Articles.DoesExist(A => A.ArticleId == articleId))
            {
                throw new RecordNotFoundException("specified article cannot be found");
            }

            if (!_unitOfWork.Comments.DoesExist(C => C.CommentId == commentId))
            {
                throw new RecordNotFoundException("specified comment cannot be found");
            }

            var comment = _unitOfWork.Comments.Get(commentId);
            var user = _unitOfWork.Users.Get(comment.UserId);
            var response = _mapper.Map<CommentResponse>((comment, user));

            return response;
        }

        public async Task<IEnumerable<CommentResponse>> GetCommentsOnArticle(int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("specified article cannot be found");
            }

            var comments = _unitOfWork.Comments.Find(c => c.ArticleId == articleId);
            if (comments.Count() == 0)
            {
                return Enumerable.Empty<CommentResponse>();
            }

            List<CommentResponse> responses = new List<CommentResponse>();
            foreach (var comment in comments)
            {
                var user = _unitOfWork.Users.Get(comment.UserId);
                var commentResponse = _mapper.Map<CommentResponse>((comment, user));
                responses.Add(commentResponse);
            }

            return responses;
        }

        public async Task<ActionResult> PostArticle(ArticlePostRequest request, UserSessionModel userFromSession)
        {
            if (userFromSession is null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }

            var article = _mapper.Map<Articles>((request, userFromSession));
            _unitOfWork.Articles.Add(article);
            _unitOfWork.complete();

            return await Task.FromResult<ActionResult>(new OkResult());
        }

        public async Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentRequest request)
        {
            if(user is null)
            {
                throw new UnauthorizedUserException("you need to re-login");
            }
            if (!_unitOfWork.Articles.DoesExist(a=> a.ArticleId == articleId))
            {
                throw new RecordNotFoundException("specified article cannot be found");
            }

            var comment = new Comments
            {
                CommentContent = request.CommentContent,
                ArticleId = articleId,
                UserId = user.UserId
            };
            _unitOfWork.Comments.Add(comment);
            _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
    }
}
