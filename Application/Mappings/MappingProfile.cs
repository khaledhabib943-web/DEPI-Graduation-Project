using AutoMapper;
using Application.DTOs;
using Domain_layer.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Favorite, FavoriteDto>()
                .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.FullName : string.Empty));
        }
    }
}
