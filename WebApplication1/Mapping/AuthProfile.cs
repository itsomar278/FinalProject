using AutoMapper;
using Domain.Models.DTO_s.RequestDto_s;
using WebApplication1.Requests;

namespace WebApplication1.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<UserRegisterRequest, UserRegisterRequestDto>()
                  .ForMember(r => r.UserEmail, m => m.MapFrom(source => source.UserEmail))
                  .ForMember(r => r.UserName, m => m.MapFrom(source => source.UserName))
                  .ForMember(r => r.Password, m => m.MapFrom(source => source.Password));

            CreateMap<UserLoginRequest, UserLoginRequestDto>();

            CreateMap<UserPasswordUpdateRequest, UserPasswordUpdateRequestDto>()
                .ForMember(r => r.UserEmail, m => m.MapFrom(source => source.UserEmail))
                .ForMember(r => r.NewPassword, m => m.MapFrom(source => source.NewPassword))
                .ForMember(r => r.OldPassword, m => m.MapFrom(source => source.OldPassword));

            CreateMap<UpdateUserNameRequest, UpdateUserNameRequestDto>();



        }
    }
}
