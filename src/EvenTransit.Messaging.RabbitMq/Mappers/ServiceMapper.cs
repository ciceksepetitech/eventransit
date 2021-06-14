using AutoMapper;
using EvenTransit.Data.Entities;
using EvenTransit.Messaging.Core.Dto;

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