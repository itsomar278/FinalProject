using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly IMapper _mapper;
        const int maxArticlesPageSize = 5;
        public ArticlesController(IUnitOfWork unitOfWork, ISessionDataManagment sessionDataManagment , IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _sessionDataManagment = sessionDataManagment;
            _mapper = mapper;
        }
        [HttpPost, Authorize]
        public ActionResult<Articles> PostArticle(ArticlePostRequest request)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            if (user==null)
            {
                return Unauthorized();
            }
            else
            {
                var article = _mapper.Map<Articles>((request , user));
                _unitOfWork.Articles.Add(article);
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
                return Ok("no articles posted yet");
            }
            else
            {
                List<ArticleResponse> articleResponses = new List<ArticleResponse>();
                foreach (var article in articles)
                {
                   var user =  _unitOfWork.Users.Get(article.UserId);
                    var response = _mapper.Map<ArticleResponse>((article, user));
                    articleResponses.Add(response);
                }
                return Ok(articleResponses);
            }
        }
        [HttpGet("{ArticleId}"), Authorize]
        public ActionResult GetArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("cannot find the specified article");
            }
            var article = _unitOfWork.Articles.Get(articleId);
            var user = _unitOfWork.Users.Get(article.UserId);
            var response = _mapper.Map<ArticleResponse>((article , user));
            return Ok(response);
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
             var articlePostRequest = _mapper.Map<ArticlePostRequest>(articleToUpdate);
            patchDocument.ApplyTo(articlePostRequest, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _mapper.Map<ArticlePostRequest, Articles>(articlePostRequest, articleToUpdate);
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
                List<CommentResponse> responses = new List<CommentResponse>();
                foreach (var comment in comments)
                {
                    var user = _unitOfWork.Users.Get(comment.UserId);
                    var commentResponse = _mapper.Map<CommentResponse>((comment , user));
                    responses.Add(commentResponse);
                }
                return Ok(responses);
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
            var user = _unitOfWork.Users.Get(comment.UserId);
            var response = _mapper.Map<CommentResponse>((comment , user));
            return Ok(response);
        }
        [HttpPost("{ArticleId}/Comments"), Authorize]
        public ActionResult<Articles> PostComments(CommentRequest request, [FromRoute(Name = "ArticleId")] int articleId)
        {
            if (!_unitOfWork.Articles.DoesExist(a => a.ArticleId == articleId))
            {
                return NotFound("specified article cannot be found");
            }
            var user = _sessionDataManagment.GetUserFromSession();
            if (user==null)
            {
                return Unauthorized();
            }
            var comment = _mapper.Map<Comments>((request, user, articleId));
            _unitOfWork.Comments.Add(comment);
            _unitOfWork.complete();
            return Ok("comment succesfully posted !");
        }
        [HttpDelete("{ArticleId}/Comments"), Authorize]
        public ActionResult DeleteCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, CommentDeleteRequest request)
        {
           if(!_unitOfWork.Comments.DoesExist(c => c.CommentId == request.commentId))
           {
                return NotFound("there is no comment with the specified id");
           }
           var comment = _unitOfWork.Comments.Get(request.commentId);
            var user = _sessionDataManagment.GetUserFromSession();
            if(user==null)
            {
                return Unauthorized("you gotta re-login");
            }
            if(user.UserId != comment.UserId)
            {
                return Unauthorized("you cant delete another user comment");
            }
            _unitOfWork.Comments.Remove(comment);
            _unitOfWork.complete();
            return Ok("comment deleted successfully");
        }
    }
}
