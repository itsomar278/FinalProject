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
using Domain.Models.Requests;

namespace Domain.Services.ArticleService
{
    public interface IArticleService
    {
        Task<ActionResult> PostArticle(ArticlePostRequest request, UserSessionModel userFromSession);
        Task<IEnumerable<ArticleResponse>> GetArticles(ArticlesSearchRequest articlesSearchRequest);
        Task<ArticleResponse> GetArticle(int articleId);
        Task<ActionResult> DeleteArticle(int articleId, UserSessionModel user);
        Task<ActionResult> ArticlePartialUpdate(int articleId, JsonPatchDocument<ArticlePostRequest> patchDocument, UserSessionModel user);

    }
}
