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
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;

namespace Domain.Services.ArticleService
{
    public interface IArticleService
    {
        Task<ActionResult> PostArticle(ArticlePostRequestDto request, UserSessionModel userFromSession);
        Task<IEnumerable<ArticleResponseDto>> GetArticles(ArticlesSearchRequestDto articlesSearchRequest);
        Task<ArticleResponseDto> GetArticle(int articleId);
        Task<ActionResult> DeleteArticle(int articleId, UserSessionModel user);
        Task<ActionResult> ArticlePartialUpdate(int articleId, JsonPatchDocument<ArticlePostRequestDto> patchDocument, UserSessionModel user);

    }
}
