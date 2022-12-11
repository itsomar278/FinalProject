using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models;
using WebApplication1.Models.Response;

namespace FinalProject.DL.Services
{
    public interface IArticleService
    {
        Task<ActionResult> PostArticle(ArticlePostRequest request, UserSessionModel userFromSession);
        Task<IEnumerable<ArticleResponse>> GetArticles(string? title, string? searchQuery, int pageNumber = 1, int pageSize = 2);
        Task<ArticleResponse> GetArticle(int articleId);
        Task<ActionResult> DeleteArticle(int articleId, UserSessionModel user);
        Task<ActionResult> ArticlePartialUpdate(int articleId, JsonPatchDocument<ArticlePostRequest> patchDocument , UserSessionModel user);
        Task<IEnumerable<CommentResponse>> GetCommentsOnArticle(int articleId);
        Task<CommentResponse> GetCommentOnArticle(int articleId, int commentId);
        Task<ActionResult> DeleteCommentOnArticle(int articleId, UserSessionModel user , CommentDeleteRequest request);
        Task<ActionResult> PostComment(int articleId, UserSessionModel user , CommentRequest request);


    }
}
