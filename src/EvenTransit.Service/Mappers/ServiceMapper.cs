using AutoMapper;
using EvenTransit.Service.Dto.Event;

namespace EvenTransit.Service.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<Data.Entities.Service, ServiceDto>();
            CreateMap<SaveServiceDto, Data.Entities.Service>()
                .ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.ServiceName));
        }
    }
}