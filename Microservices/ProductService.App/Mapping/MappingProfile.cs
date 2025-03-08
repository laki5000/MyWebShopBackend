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
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<ApiResponseDto, GrpcResponseDto>();
        }
    }
}
