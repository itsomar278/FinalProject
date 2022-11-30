using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.UnitOfWorks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ArticlesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("PostArticle") , Authorize]
        public async Task<ActionResult<Articles>> PostArticle(ArticlePostRequest request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if(string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }
            else
            {
                var user =_unitOfWork.Users.FindByEmail(userEmail);
                Articles Article = new Articles
                {
                    Title = request.Title ,
                    Content= request.Content ,
                    UserId = user.UserId
                };
                _unitOfWork.Articles.Add(Article);
                _unitOfWork.complete();
                return Ok("article posted");
            }
        }
        [HttpGet,Authorize]
        public async Task<ActionResult<Articles>> GetArticles(string? title , string? searchQuery)
        {
            var articles = _unitOfWork.Articles.GetArticles( title, searchQuery);
            if(articles.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                List<ArticleResponse> articleResponses = new List<ArticleResponse>();
                foreach (var article in articles)
                {
                    ArticleResponse ArticleResponse = new ArticleResponse
                    {
                        AuthorUserName = _unitOfWork.Users.Find(u => u.UserId == article.UserId).First().UserName,
                        Title = article.Title ,
                        Content = article.Content ,
                    };
                    articleResponses.Add(ArticleResponse);
                }
                return Ok(articleResponses);
            }
        }
    }
}
