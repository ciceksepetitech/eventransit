using AutoMapper;
using EvenTransit.Domain.Entities;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.RabbitMq.Mappers
{
    public class EventLogMapper : Profile
    {
        public EventLogMapper()
        {
            CreateMap<EventLogDto, Logs>();
            CreateMap<EventDetailDto, LogDetail>();
            CreateMap<HttpRequestDto, LogDetailRequest>();
        }
    }
}