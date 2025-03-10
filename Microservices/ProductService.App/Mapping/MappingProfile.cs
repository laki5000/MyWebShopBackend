using AutoMapper;
using Productservice.Proto;
using ProductService.App.Models;
using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ProductService.App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GrpcCreateCategoryDto, CreateCategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<GrpcUpdateCategoryDto, UpdateCategoryDto>();
            CreateMap<GrpcDeleteCategoryDto, DeleteCategoryDto>();
            CreateMap<Category, GetCategoryDto>();
            CreateMap<ApiResponseDto, GrpcResponseDto>();
        }
    }
}
