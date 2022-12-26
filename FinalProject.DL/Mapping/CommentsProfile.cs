using AutoMapper;
using WebApplication1.Models;
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
        }

    }
}
