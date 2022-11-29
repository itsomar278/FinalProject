using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.Entites;
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
    }
}
