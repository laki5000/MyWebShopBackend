using AutoMapper;
using ProductService.App.Models;
using ProductService.Shared.Dtos;

namespace ProductService.App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}
