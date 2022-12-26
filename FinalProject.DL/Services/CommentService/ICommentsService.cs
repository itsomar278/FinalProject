using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;
using WebApplication1.Models;

namespace Domain.Services.CommentService
{
    public interface ICommentsService
    {
        Task<IEnumerable<CommentResponse>> GetCommentsOnArticle(int articleId);
        Task<CommentResponse> GetCommentOnArticle(int articleId, int commentId);
        Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user, CommentDeleteRequest request);
        Task<ActionResult> PostComment(int articleId, UserSessionModel user, CommentRequest request);
    }
}
