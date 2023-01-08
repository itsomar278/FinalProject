using AutoMapper;
using Contracts.Requests;
using Contracts.Response;
using DataAcess.Entites;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;

namespace WebApplication1.Mapping
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<(Comments comment, Users user), CommentResponseDto>()
            .ForMember(cr => cr.UserName, m => m.MapFrom(source => source.user.UserName))
            .ForMember(cr => cr.CommentContent, m => m.MapFrom(source => source.comment.CommentContent));

            CreateMap<CommentResponseDto, CommentResponse>();
            CreateMap<CommentPostRequest, CommentPostRequestDto>();
        }

    }
}
