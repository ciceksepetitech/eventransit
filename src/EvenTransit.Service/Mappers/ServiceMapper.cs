using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;

namespace EvenTransit.Service.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<Core.Entities.Service, ServiceDto>();
            CreateMap<SaveServiceDto, Core.Entities.Service>();
        }
    }
}