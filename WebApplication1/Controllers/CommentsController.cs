using AutoMapper;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Services.CommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Services.Session;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly ICommentsService _commentsService;
        private readonly IMapper _mapper;
        public CommentsController(ISessionDataManagment sessionDataManagment, ICommentsService commentsService, IMapper Mapper)
        {
            _sessionDataManagment = sessionDataManagment;
            _commentsService = commentsService;
            _mapper = Mapper;
        }

        [HttpGet("{ArticleId}/Comments")]
        public async Task<ActionResult<CommentResponse>> GetCommentsOnArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var responsesDtos = await _commentsService.GetCommentsOnArticle(articleId);
            if (responsesDtos.Count() == 0)
            {
                return Ok("there is no comments on this article");
            }
            var responses = _mapper.Map<List<CommentResponse>>(responsesDtos);
            return Ok(responses);
        }

        [HttpGet("{ArticleId}/Comments/{CommentId}")]
        public async Task<ActionResult<CommentResponse>> GetCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, [FromRoute(Name = "CommentId")] int commentId)
        {
            var response = await _commentsService.GetCommentOnArticle(articleId, commentId);

            return Ok(response);
        }

        [HttpPost("{ArticleId}/Comments"), Authorize]
        public async Task<ActionResult<Articles>> PostComment(CommentPostRequest request, [FromRoute(Name = "ArticleId")] int articleId)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            var requestDto = _mapper.Map<CommentPostRequestDto>(request);
            await _commentsService.PostComment(articleId, user, requestDto);

            return Ok("comment successfully posted");
        }

        [HttpDelete("{ArticleId}/Comments"), Authorize]
        public async Task<ActionResult> DeleteCommentOnArticle([FromRoute(Name = "ArticleId")] int articleId, int commentId)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _commentsService.DeleteCommentOnArticle(articleId, user, commentId);

            return Ok("comment deleted successfully");
        }
    }
}
