using AutoMapper;
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
            CreateMap<(ArticlePostRequest request, UserSessionModel user), Articles>()
                   .ForMember(a => a.UserId, m => m.MapFrom(source => source.user.UserId))
                   .ForMember(a => a.Content, m => m.MapFrom(source => source.request.Content))
                   .ForMember(sss => sss.Title, m => m.MapFrom(source => source.request.Title));

            CreateMap<(Articles article , Users user), ArticleResponse>()
                  .ForMember(a => a.AuthorUserName, m => m.MapFrom(source => source.user.UserName))
                  .ForMember(a => a.Content, m => m.MapFrom(source => source.article.Content))
                  .ForMember(a => a.Title, m => m.MapFrom(source => source.article.Title));

            CreateMap<Articles , ArticlePostRequest>()
                .ForMember(ap => ap.Title, m => m.MapFrom(source => source.Title))
                .ForMember(ap => ap.Content, m => m.MapFrom(source => source.Content))
                .ReverseMap();
        }
    }
}
