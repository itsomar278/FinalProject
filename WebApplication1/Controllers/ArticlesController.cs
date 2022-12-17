using AutoMapper;
using Domain.Services.ArticlesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.UnitOfWorks;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Services.SessionManagment;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly IArticleService _articleService;
        const int maxArticlesPageSize = 5;
        public ArticlesController( ISessionDataManagment sessionDataManagment, IArticleService articleService)
        {
            _sessionDataManagment = sessionDataManagment;
            _articleService = articleService;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> PostArticle(ArticlePostRequest request)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.PostArticle(request, user);
            return Ok("article posted !");
        }

        [HttpGet]
        public async Task<ActionResult<ArticleResponse>> GetArticles(string? title, string? searchQuery, int pageNumber = 1, int pageSize = 2)
        {
            if (pageSize > maxArticlesPageSize)
            {
                pageSize = maxArticlesPageSize;
            }

            var articles = await _articleService.GetArticles(title, searchQuery, pageNumber, pageSize);
            if (articles.Count() == 0)
            {
                return Ok("no articles posted yet");
            }

            return Ok(articles);
        }

        [HttpGet("{ArticleId}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var response = await _articleService.GetArticle(articleId);
            return Ok(response);
        }

        [HttpDelete("{ArticleId}"), Authorize]
        public async Task<ActionResult> DeleteArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.DeleteArticle(articleId, user);
            return Ok("Article deleted");
        }

        [HttpPatch("{ArticleId}"), Authorize]
        public async Task<ActionResult> ArticlePartialUpdate([FromRoute(Name = "ArticleId")] int articleId
            , JsonPatchDocument<ArticlePostRequest> patchDocument)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.ArticlePartialUpdate(articleId, patchDocument , user);

            return Ok("Article successfully updated");
        }

        [HttpGet("{ArticleId}/Comments")]
        public async Task<ActionResult<CommentResponse>> GetCommentsOnArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var responses = await _articleService.GetCommentsOnArticle(articleId);
            if (responses.Count() == 0)
            {
                return Ok("there is no comments on this article");
            }

            return Ok(responses);
        }

        [HttpGet("{ArticleId}/Comments/{CommentId}")]
        public async Task<ActionResult<CommentResponse>> GetCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, [FromRoute(Name = "CommentId")] int commentId)
        {
            var response = await _articleService.GetCommentOnArticle(articleId, commentId);

            return Ok(response);
        }

        [HttpPost("{ArticleId}/Comments"), Authorize]
        public async Task<ActionResult<Articles>> PostComment(CommentRequest request, [FromRoute(Name = "ArticleId")] int articleId)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.PostComment(articleId, user, request);

            return Ok("comment successfully posted");
        }

        [HttpDelete("{ArticleId}/Comments"), Authorize]
        public async Task<ActionResult> DeleteCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, CommentDeleteRequest request)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.DeleteCommentOnArticle(articleId, user, request);

            return Ok("comment deleted successfully");
        }
    }
}
