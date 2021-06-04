using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.UI.Models.Events;

namespace EvenTransit.UI.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<EventDto, Core.Dto.UI.EventDto>();
            CreateMap<ServiceDto, Core.Dto.UI.ServiceDto>();
        }
    }
}