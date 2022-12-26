
using Domain.Services.ArticleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Services.Session;

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
        public async Task<ActionResult<ArticleResponse>> GetArticles(string? title, string? searchQuery, int pageNumber = 1, int pageSize = 2) // create request contains these params 
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
    }
}
