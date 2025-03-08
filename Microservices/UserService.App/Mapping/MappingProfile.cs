using AutoMapper;
using Shared.Dtos;
using Userservice.Proto;
using UserService.App.Models;
using UserService.Shared.Dtos;

namespace UserService.App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<GrpcCreateUserDto, CreateUserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<ApiResponseDto, GrpcResponseDto>();
        }
    }
}
