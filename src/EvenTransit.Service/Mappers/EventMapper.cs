using AutoMapper;
using EvenTransit.Data.Entities;
using EvenTransit.Service.Dto.Event;

namespace EvenTransit.Service.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id))
                .ReverseMap();
        }
    }
}