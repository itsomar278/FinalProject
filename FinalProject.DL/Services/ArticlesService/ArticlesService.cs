using AutoMapper;
using Domain.Services.ArticlesService;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace Domain.Services.ArticlesService
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

        public async Task<ActionResult> ArticlePartialUpdate(int articleId, JsonPatchDocument<ArticlePostRequest> patchDocument, UserSessionModel user)
        {
            if (user is null)
                throw new UnauthorizedUserException("you need to relogin");
            
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("cannot find the specifed article");  

            var articleToUpdate = await _unitOfWork.Articles.GetAsync(articleId);

            if (user.UserId != articleToUpdate.UserId)
                throw new UnauthorizedUserException("you cant upadte other users articles");

            var articlePostRequest = _mapper.Map<ArticlePostRequest>(articleToUpdate);
            patchDocument.ApplyTo(articlePostRequest);
            _mapper.Map<ArticlePostRequest,Articles>(articlePostRequest, articleToUpdate);

            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ActionResult> DeleteArticle(int articleId, UserSessionModel user)
        {
            if (user == null)
                throw new UnauthorizedUserException("you need to relogin");
            
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
               throw new RecordNotFoundException("cannot find the specifed article");
            
            var article = await _unitOfWork.Articles.GetAsync(articleId);

            if (article.UserId != user.UserId)
                throw new UnauthorizedUserException("you cant delete this article ");

            var related =await _unitOfWork.Favorites.FindAsync(f => f.ArticleId == articleId); // to avoid cycles and multi cascade pathes 
            _unitOfWork.Favorites.RemoveRange(related);
            await _unitOfWork.complete();

            _unitOfWork.Articles.Remove(article);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user, CommentDeleteRequest request)
        {
            if (user is null)
               throw new UnauthorizedUserException("you need to re-login");
            
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
               throw new RecordNotFoundException("there is no article with the specified id");
            
            if (!await _unitOfWork.Comments.DoesExistAsync(c => c.CommentId == request.commentId))
               throw new RecordNotFoundException("there is no comment with the specified id");

            var comment = await _unitOfWork.Comments.GetAsync(request.commentId);

            if (user.UserId != comment.UserId)
                throw new UnauthorizedUserException("you cant delete another user comment");

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }

        public async Task<ArticleResponse> GetArticle(int articleId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("cannot find the specifed article");

            var article = await _unitOfWork.Articles.GetAsync(articleId);
            var user = await _unitOfWork.Users.GetAsync(article.UserId);
            var response = _mapper.Map<ArticleResponse>((article, user));

            return await Task.FromResult(response);
        }


        public async Task<IEnumerable<ArticleResponse>> GetArticles(string? title, string? searchQuery, int pageNumber, int pageSize)
        {
            var articles = await _unitOfWork.Articles.GetArticlesAsync(title, searchQuery, pageNumber, pageSize);

            if (articles.Count() == 0)
               return Enumerable.Empty<ArticleResponse>();
            
            List<ArticleResponse> articleResponses = new List<ArticleResponse>();

            foreach (var article in articles)
            {
                var user = await _unitOfWork.Users.GetAsync(article.UserId);
                var response = _mapper.Map<ArticleResponse>((article, user));
                articleResponses.Add(response);
            }

            return await Task.FromResult(articleResponses);
        }

        public async Task<CommentResponse> GetCommentOnArticle(int articleId, int commentId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(A => A.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");
            
            if (!await _unitOfWork.Comments.DoesExistAsync(C => C.CommentId == commentId))
               throw new RecordNotFoundException("specified comment cannot be found");

            var comment = await _unitOfWork.Comments.GetAsync(commentId);
            var user = await _unitOfWork.Users.GetAsync(comment.UserId);
            var response = _mapper.Map<CommentResponse>((comment, user));

            return await Task.FromResult(response);
        }

        public async Task<IEnumerable<CommentResponse>> GetCommentsOnArticle(int articleId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("specified article cannot be found");
            

            var comments = await _unitOfWork.Comments.FindAsync(c => c.ArticleId == articleId);
            if (comments.Count() == 0)
                return Enumerable.Empty<CommentResponse>();
            

            List<CommentResponse> responses = new List<CommentResponse>();
            foreach (var comment in comments)
            {
                var user = await _unitOfWork.Users.GetAsync(comment.UserId);
                var commentResponse = _mapper.Map<CommentResponse>((comment, user));
                responses.Add(commentResponse);
            }

            return await Task.FromResult(responses);
        }

        public async Task<ActionResult> PostArticle(ArticlePostRequest request, UserSessionModel userFromSession)
        {
            if (userFromSession is null)
                throw new UnauthorizedUserException("you need to re-login");
            
            var article = _mapper.Map<Articles>((request, userFromSession));
            _unitOfWork.Articles.AddAsync(article);
            await _unitOfWork.complete();

            return await Task.FromResult<ActionResult>(new OkResult());
        }

        public async Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentRequest request)
        {
            if (user is null)
               throw new UnauthorizedUserException("you need to re-login");
            
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
               throw new RecordNotFoundException("specified article cannot be found");
            
            var comment = new Comments
            {
                CommentContent = request.CommentContent,
                ArticleId = articleId,
                UserId = user.UserId
            };

            _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.complete();

            return await Task.FromResult(new OkResult());
        }
    }
}
