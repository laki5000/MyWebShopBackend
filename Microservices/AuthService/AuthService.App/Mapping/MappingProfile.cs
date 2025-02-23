using Auth;
using AuthService.Models;
using AuthService.Shared.Dtos;
using AutoMapper;
using Shared.Dtos;

namespace AuthService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GrpcCreateAspNetUserDto, CreateAspNetUserDto>();
            CreateMap<GrpcLoginAspNetUserDto, LoginAspNetUserDto>();
            CreateMap<CreateAspNetUserDto, AspNetUser>();
            CreateMap<AspNetUser, AspNetUserDto>();
            CreateMap<AspNetUserDto, GrpcAspNetUserDto>();
            CreateMap<ApiResponseDto<AspNetUserDto>, GrpcAspNetUserResponseDto>();
            CreateMap<ApiResponseDto<string>, GrpcStringResponseDto>();
        }
    }
}
