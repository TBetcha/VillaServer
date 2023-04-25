using AutoMapper;
using VillaAPI.Models;
using VillaAPI.Models.DTO;

namespace VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDto>();
        CreateMap<VillaDto, Villa>();
        CreateMap<Villa, VillaCreateDto>().ReverseMap();
        CreateMap<Villa, VillaUpdateDto>().ReverseMap();
    }

}