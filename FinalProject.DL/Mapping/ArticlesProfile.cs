using AutoMapper;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Models.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Security.Principal;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace WebApplication1.Mapping
{
    public class ArticlesProfile: Profile
    {
        public ArticlesProfile()
        {
            CreateMap<(ArticlePostRequestDto request, UserSessionModel user), Articles>()
                   .ForMember(a => a.UserId, m => m.MapFrom(source => source.user.UserId))
                   .ForMember(a => a.Content, m => m.MapFrom(source => source.request.Content))
                   .ForMember(sss => sss.Title, m => m.MapFrom(source => source.request.Title));

            CreateMap<(Articles article , Users user), ArticleResponseDto>()
                  .ForMember(a => a.AuthorUserName, m => m.MapFrom(source => source.user.UserName))
                  .ForMember(a => a.Content, m => m.MapFrom(source => source.article.Content))
                  .ForMember(a => a.Title, m => m.MapFrom(source => source.article.Title));

            CreateMap<Articles , ArticlePostRequestDto>()
                .ForMember(ap => ap.Title, m => m.MapFrom(source => source.Title))
                .ForMember(ap => ap.Content, m => m.MapFrom(source => source.Content))
                .ReverseMap();

            CreateMap<ArticlePostRequest, ArticlePostRequestDto>()
                .ForMember(d => d.Content, m => m.MapFrom(source => source.Content))
                .ForMember(d => d.Title, m => m.MapFrom(source => source.Title));

            CreateMap<ArticlesSearchRequest, ArticlesSearchRequestDto>();

            CreateMap<JsonPatchDocument<ArticlePostRequest>, JsonPatchDocument<ArticlePostRequestDto>>();

            CreateMap<Operation<ArticlePostRequest>, Operation<ArticlePostRequestDto>>();

            CreateMap<ArticleResponseDto , ArticleResponse>();
        }
    }
}
