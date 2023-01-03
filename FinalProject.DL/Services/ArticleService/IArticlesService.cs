using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Services.SessionService;

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
