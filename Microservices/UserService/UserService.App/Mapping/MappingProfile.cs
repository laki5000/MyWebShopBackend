using AutoMapper;
using Shared.Dtos;
using User;
using UserService.App.Models;
using UserService.Shared.Dtos;

namespace UserService.App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<GrpcCreateUserDto, CreateUserDto>();
            CreateMap<CreateUserDto, UserEntity>();
            CreateMap<ApiResponseDto, GrpcResponseDto>();
        }
    }
}
