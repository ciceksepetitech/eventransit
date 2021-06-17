using AutoMapper;
using EvenTransit.Domain.Entities;
using EvenTransit.Service.Dto.Event;

namespace EvenTransit.Service.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<Event, EventDto>().ReverseMap();

            CreateMap<SaveEventDto, Event>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.EventName));
        }
    }
}