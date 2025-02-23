using Auth;
using AuthService.Shared.Dtos;
using AutoMapper;
using Shared.Dtos;

namespace ApiGateway.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAspNetUserDto, GrpcCreateAspNetUserDto>();
            CreateMap<LoginAspNetUserDto, GrpcLoginAspNetUserDto>();
            CreateMap<GrpcAspNetUserDto, AspNetUserDto>();
            CreateMap<GrpcAspNetUserResponseDto, ApiResponseDto<AspNetUserDto>>();
            CreateMap<GrpcStringResponseDto, ApiResponseDto<string>>();
        }
    }
}
