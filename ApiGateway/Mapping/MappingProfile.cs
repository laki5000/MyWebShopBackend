using Auth;
using AuthService.Shared.Dtos;
using AutoMapper;
using Shared.Dtos;
using User;
using UserService.Shared.Dtos;

namespace ApiGateway.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAspNetUserDto, GrpcCreateAspNetUserDto>();
            CreateMap<LoginAspNetUserDto, GrpcLoginAspNetUserDto>();
            CreateMap<CreateUserDto, GrpcCreateUserDto>();
            CreateMap<GrpcStringResponseDto, ApiResponseDto<string>>();
            CreateMap<GrpcStringListResponseDto, ApiResponseDto<List<string>>>();
            CreateMap<GrpcResponseDto, ApiResponseDto>();
        }
    }
}
