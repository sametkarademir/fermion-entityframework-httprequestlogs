using AutoMapper;
using WebApi.DTOs.Categories;
using WebApi.Entities;

namespace WebApi.Profiles;

public class EntityProfiles : Profile
{
    public EntityProfiles()
    {
        CreateMap<Category, CategoryResponseDto>();
    }
}