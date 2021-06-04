using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Entities;

namespace EvenTransit.Messaging.RabbitMq.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<Service, ServiceDto>();
        }
    }
}