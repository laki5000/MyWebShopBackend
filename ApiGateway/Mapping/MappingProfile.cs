using Authservice.Proto;
using AuthService.Shared.Dtos;
using AutoMapper;
using Productservice.Proto;
using ProductService.Shared.Dtos;
using Shared.Dtos;
using Userservice.Proto;
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
            CreateMap<ChangeAspNetUserPasswordDto, GrpcChangeAspNetUserPasswordDto>();
            CreateMap<CreateCategoryDto, GrpcCreateCategoryDto>();
            CreateMap<UpdateCategoryDto, GrpcUpdateCategoryDto>();
            CreateMap<DeleteCategoryDto, GrpcDeleteCategoryDto>();
            CreateMap<CreateProductDto, GrpcCreateProductDto>();
            CreateMap<UpdateProductDto, GrpcUpdateProductDto>();
            CreateMap<DeleteProductDto, GrpcDeleteProductDto>();
            CreateMap<GrpcStringResponseDto, ApiResponseDto<string>>();
            CreateMap<GrpcStringListResponseDto, ApiResponseDto<List<string>>>();
            CreateMap<GrpcGetCategoryDto, GetCategoryDto>();
            CreateMap<Authservice.Proto.GrpcResponseDto, ApiResponseDto>();
            CreateMap<Userservice.Proto.GrpcResponseDto, ApiResponseDto>();
            CreateMap<Productservice.Proto.GrpcResponseDto, ApiResponseDto>();
            CreateMap<GrpcGetCategoryDtoListResponseDto, ApiResponseDto<List<GetCategoryDto>>>();
        }
    }
}
