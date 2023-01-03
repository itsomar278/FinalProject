using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Models;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.DTO_s.RequestDto_s;

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
