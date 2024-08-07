﻿
using AutoMapper;
using Contracts.Requests;
using Contracts.Response;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Services.ArticleService;
using Domain.Services.SessionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ISessionDataManagment _sessionDataManagment;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        const int maxArticlesPageSize = 5;
        public ArticlesController( ISessionDataManagment sessionDataManagment, IArticleService articleService , IMapper mapper)
        {
            _sessionDataManagment = sessionDataManagment;
            _articleService = articleService;
            _mapper = mapper;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> PostArticle(ArticlePostRequest request)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            var RequestDto = _mapper.Map<ArticlePostRequestDto>(request);
            await _articleService.PostArticle(RequestDto, user);

            return Ok("article posted !");
        }

        [HttpGet]
        public async Task<ActionResult<ArticleResponse>> GetArticles([FromQuery] ArticlesSearchRequest articlesSearchRequest) 
        {
            if (articlesSearchRequest.pageSize > maxArticlesPageSize)
            {
                articlesSearchRequest.pageSize = maxArticlesPageSize;
            }
            var articlesSearchRequestDto = _mapper.Map<ArticlesSearchRequestDto>(articlesSearchRequest);

            var articlesResponseDtos = await _articleService.GetArticles(articlesSearchRequestDto);
            if (articlesResponseDtos.Count() == 0)
            {
                return Ok("no articles posted yet");
            }

            var response = _mapper.Map<List<ArticleResponse>>(articlesResponseDtos);
            return Ok(response);
        }

        [HttpGet("{ArticleId}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var articleDto = await _articleService.GetArticle(articleId);
            var response = _mapper.Map<ArticleResponse>(articleDto);
            return Ok(response);
        }

        [HttpDelete("{ArticleId}"), Authorize]
        public async Task<ActionResult> DeleteArticle([FromRoute(Name = "ArticleId")] int articleId)
        {
            var user = _sessionDataManagment.GetUserFromSession();
            await _articleService.DeleteArticle(articleId, user);
            return Ok("Article deleted");
        }

        [HttpPatch("{ArticleId}"), Authorize]
        public async Task<ActionResult> ArticlePartialUpdate([FromRoute(Name = "ArticleId")] int articleId
            , JsonPatchDocument<ArticlePostRequest> patchDocument)
        {
            var user = _sessionDataManagment.GetUserFromSession();

            var documentDto = _mapper.Map<JsonPatchDocument<ArticlePostRequestDto>>(patchDocument);

            await _articleService.ArticlePartialUpdate(articleId, documentDto, user);

            return Ok("Article successfully updated");
        }
    }
}
