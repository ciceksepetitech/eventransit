using AutoMapper;
using EvenTransit.Core.Dto.Service;

namespace EvenTransit.Service.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<Core.Entities.Service, ServiceDto>();
        }
    }
}