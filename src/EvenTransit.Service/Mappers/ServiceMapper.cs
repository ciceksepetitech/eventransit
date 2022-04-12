using AutoMapper;
using EvenTransit.Service.Dto.Event;

namespace EvenTransit.Service.Mappers;

public class ServiceMapper : Profile
{
    public ServiceMapper()
    {
        CreateMap<Domain.Entities.Service, ServiceDto>();
        CreateMap<SaveServiceDto, Domain.Entities.Service>()
            .ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.ServiceName));
    }
}
