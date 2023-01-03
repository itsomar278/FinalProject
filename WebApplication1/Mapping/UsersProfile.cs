using AutoMapper;
using DataAcess.Entites;
using Domain.Models.DTO_s.RequestDto_s;
using Domain.Models.DTO_s.ResponseDto_s;
using Domain.Services.SessionService;
using WebApplication1.Requests;
using WebApplication1.Response;

namespace WebApplication1.Mapping
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<(UserRegisterRequestDto request, byte[] PasswordHash, byte[] passwordSalt), Users>()
                .ForMember(u => u.UserName, m => m.MapFrom(source => source.request.UserName))
                .ForMember(u => u.UserEmail, m => m.MapFrom(source => source.request.UserEmail))
                .ForMember(u => u.PasswordHash, m => m.MapFrom(source => source.PasswordHash))
                .ForMember(u => u.PasswordSalt, m => m.MapFrom(source => source.passwordSalt));

            CreateMap<Users, UserResponseDto>()
                .ForMember(ur => ur.UserName, m => m.MapFrom(source => source.UserName))
                .ForMember(ur => ur.UserEmail, m => m.MapFrom(source => source.UserEmail));

            CreateMap<(FollowRequest request, int userId), Follow>()
                .ForMember(f => f.FollowedId, m => m.MapFrom(source => source.request.UserToFollowId))
                .ForMember(f => f.FollowerId, m => m.MapFrom(source => source.userId));

            CreateMap<(AddToFavouritesRequestDto request, int userId), Favorite>()
                .ForMember(f => f.UserId, m => m.MapFrom(source => source.userId))
                .ForMember(f => f.ArticleId, m => m.MapFrom(source => source.request.ArticleId));

            CreateMap<Users, UserSessionModel>()
                .ForMember(us => us.UserName, m => m.MapFrom(source => source.UserName))
                .ForMember(us => us.UserEmail, m => m.MapFrom(source => source.UserEmail))
                .ForMember(us => us.UserId, m => m.MapFrom(source => source.UserId));

            CreateMap<UserSearchRequest, UserSearchRequestDto>();
            CreateMap<UserResponseDto, UsersResponse>();
            CreateMap<FollowRequest, FollowRequestDto>();
            CreateMap<UnfollowRequest, UnfollowRequestDto>();
            CreateMap<RemoveFollowerRequest, RemoveFollowerRequestDto>();
            CreateMap<PagingRequest, PagingRequestDto>();
            CreateMap<AddToFavouritesRequest, AddToFavouritesRequestDto>();
        }
    }
}
