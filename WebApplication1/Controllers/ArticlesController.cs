using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Services.SessionManagment;
using WebApplication1.UnitOfWorks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionDataManagment _sessionDataManagment;
        const int maxArticlesPageSize = 5;
        public ArticlesController(IUnitOfWork unitOfWork, ISessionDataManagment sessionDataManagment)
        {
            _unitOfWork = unitOfWork;
            _sessionDataManagment = sessionDataManagment;
        }
        [HttpPost, Authorize]
        public ActionResult<Articles> PostArticle(ArticlePostRequest request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }
            else
            {
                var user = _unitOfWork.Users.FindByEmail(userEmail);
                Articles Article = new Articles
                {
                    Title = request.Title,
                    Content = request.Content,
                    UserId = user.UserId
                };
                _unitOfWork.Articles.Add(Article);
                _unitOfWork.complete();
                user.PublishedArticles.Add(Article);
                _unitOfWork.complete();
                return Ok("Article posted");
            }
        }
        [HttpGet, Authorize]
        public ActionResult<Articles> GetArticles(string? title, string? searchQuery, int pageNumber = 1, int pageSize = 2)
        {
            if (pageSize > maxArticlesPageSize)
            {
                pageSize = maxArticlesPageSize;
            }
            var articles = _unitOfWork.Articles.GetArticles(title, searchQuery, pageNumber, pageSize);
            if (articles.Count() == 0)
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
                        Title = article.Title,
                        Content = article.Content,
                    };
                    articleResponses.Add(ArticleResponse);
                }
                return Ok(articleResponses);
            }
        }
        [HttpGet("{ArticleId}"), Authorize]
        public ActionResult GetArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            if (_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("cannot find the specified article");
            }
            var article = _unitOfWork.Articles.Get(articleId);
            ArticleResponse ArticleResponse = new ArticleResponse
            {
                AuthorUserName = _unitOfWork.Users.Find(u => u.UserId == article.UserId).First().UserName,
                Title = article.Title,
                Content = article.Content,
            };
            return Ok(ArticleResponse);
        }
        [HttpDelete("{ArticleId}"), Authorize]
        public ActionResult DeleteArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("there is no article with such id ");
            }
            var user = _sessionDataManagment.GetUserFromSession();
            var articleToDelete = _unitOfWork.Articles.Get(articleId);
            if (user.UserId != articleToDelete.UserId)
            {
                return Unauthorized();
            }
            _unitOfWork.Articles.Remove(articleToDelete);
            _unitOfWork.complete();
            return Ok("Article succesfully deleted");
        }
        [HttpPatch("{ArticleId}"), Authorize]
        public ActionResult ArticlePartialUpdate([FromRoute(Name = "ArticleId")] int articleId
            , JsonPatchDocument<ArticlePostRequest> patchDocument)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound();
            }
            var articleToUpdate = _unitOfWork.Articles.Get(articleId);
            var user = _sessionDataManagment.GetUserFromSession();
            if (user.UserId != articleToUpdate.UserId)
            {
                return Unauthorized();
            }
            ArticlePostRequest articlePostRequest = new ArticlePostRequest
            {
                Title = articleToUpdate.Title,
                Content = articleToUpdate.Content
            };
            patchDocument.ApplyTo(articlePostRequest, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            articleToUpdate.Title = articlePostRequest.Title;
            articleToUpdate.Content = articlePostRequest.Content;
            _unitOfWork.complete();
            return Ok("Article successfully updated");
        }

        [HttpGet("{ArticleId}/Comments"), Authorize]
        public ActionResult<Comments> GetCommentsOnArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("specified article cannot be found");
            }
            else
            {
                var comments = _unitOfWork.Comments.Find(c => c.ArticleId == articleId);
                if (comments.Count() == 0)
                {
                    return Ok("there is no comments on this article ");
                }
                List<CommentResponse> response = new List<CommentResponse>();
                foreach (var comment in comments)
                {
                    CommentResponse commentResponse = new CommentResponse
                    {
                        UserName = _unitOfWork.Users.Find(u => u.UserId == comment.UserId).First().UserName,
                        CommentContent = comment.CommentContent
                    };
                    response.Add(commentResponse);
                }
                return Ok(response);
            }
        }
        [HttpGet("{ArticleId}/Comments/{CommentId}"), Authorize]
        public ActionResult getCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, [FromRoute(Name = "CommentId")] int commentId)
        { 
            if (!_unitOfWork.Articles.DoesExist(A => A.ArticleId == articleId))
            {
                return NotFound("specified article cannot be found");
            }
            if (!_unitOfWork.Comments.DoesExist(C => C.CommentId == commentId))
            {
                return Ok("the specified comment cannot be found ");
            }
            var comment = _unitOfWork.Comments.Get(commentId);
            CommentResponse response = new CommentResponse
            {
                CommentContent = comment.CommentContent,
                UserName = _unitOfWork.Users.Get(comment.UserId).UserName
            };
            return Ok(response);
        }
        [HttpPost("{ArticleId}/Comments"), Authorize]
        public ActionResult<Articles> PostComments(CommentRequest request, [FromRoute(Name = "ArticleId")] int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("specified article cannot be found");
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }
            var user = _unitOfWork.Users.FindByEmail(userEmail);
            Comments comment = new Comments
            {
                UserId = user.UserId,
                ArticleId = articleId,
                CommentContent = request.CommentContent,
            };
            _unitOfWork.Comments.Add(comment);
            _unitOfWork.complete();
            return Ok("comment succesfully posted !");
        }
        [HttpDelete("{ArticleId}/Comments/{CommentId}"), Authorize]
        public ActionResult DeleteCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, [FromRoute(Name = "CommentId")] int commentId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }
            var user = _unitOfWork.Users.FindByEmail(userEmail);
            var commentToDelete = _unitOfWork.Comments.Get(commentId);
            if (commentToDelete == null || commentToDelete.ArticleId != articleId)
            {
                return NotFound("cannot find the specified comment on this article");
            }
            if (user.UserId != commentToDelete.UserId)
            {
                return Unauthorized("you cant delete other user comments !");
            }
            _unitOfWork.Comments.Remove(commentToDelete);
            _unitOfWork.complete();
            return Ok("comment deleted succesfully");
        }
    }
}
