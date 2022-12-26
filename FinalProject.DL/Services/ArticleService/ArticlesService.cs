using AutoMapper;
using Domain.Models.Requests;
using Domain.Services.ArticleService;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace Domain.Services.ArticleService
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
            _mapper.Map(articlePostRequest, articleToUpdate);

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

        public async Task<ArticleResponse> GetArticle(int articleId)
        {
            if (!await _unitOfWork.Articles.DoesExistAsync(a => a.ArticleId == articleId))
                throw new RecordNotFoundException("cannot find the specifed article");

            var article = await _unitOfWork.Articles.GetAsync(articleId);
            var user = await _unitOfWork.Users.GetAsync(article.UserId);
            var response = _mapper.Map<ArticleResponse>((article, user));

            return await Task.FromResult(response);
        }


        public async Task<IEnumerable<ArticleResponse>> GetArticles(ArticlesSearchRequest articlesSearchRequest)
        {
            var articles = await _unitOfWork.Articles.GetArticlesAsync(articlesSearchRequest.title, articlesSearchRequest.searchQuery, articlesSearchRequest.pageNumber, articlesSearchRequest.pageSize);

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
        public async Task<ActionResult> PostArticle(ArticlePostRequest request, UserSessionModel userFromSession)
        {
            if (userFromSession is null)
               throw new UnauthorizedUserException("you need to re-login");
            
            var article = _mapper.Map<Articles>((request, userFromSession));
            _unitOfWork.Articles.AddAsync(article);
            await _unitOfWork.complete();

            return await Task.FromResult<ActionResult>(new OkResult());
        }

      
    }
}
