using Microsoft.AspNetCore.Mvc;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Services.SessionService;

namespace Domain.Services.CommentService
{
    public interface ICommentsService
    {
        Task<IEnumerable<CommentResponseDto>> GetCommentsOnArticle(int articleId);
        Task<CommentResponseDto> GetCommentOnArticle(int articleId, int commentId);
        Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user, int commentId);
        Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentPostRequestDto request);
    }
}
