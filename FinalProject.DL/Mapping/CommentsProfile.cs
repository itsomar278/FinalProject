using AutoMapper;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Requests;
using WebApplication1.Models.Response;

namespace WebApplication1.Mapping
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<(Comments comment, Users user), CommentResponse>()
           .ForMember(cr => cr.UserName, m => m.MapFrom(source => source.user.UserName))
           .ForMember(cr => cr.CommentContent, m => m.MapFrom(source => source.comment.CommentContent));
            CreateMap<(CommentRequest request, Users user, int articleId), Comments>()
            .ForMember(c => c.UserId, m => m.MapFrom(source => source.user.UserId))
            .ForMember(c => c.CommentContent, m => m.MapFrom(source => source.request.CommentContent))
            .ForMember(c => c.ArticleId, m => m.MapFrom(source => source.articleId));
        }

    }
}
