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
            CreateMap<ApiResponseDto<string>, GrpcStringResponseDto>();
            CreateMap<ApiResponseDto<List<string>>, GrpcStringListResponseDto>();
        }
    }
}
