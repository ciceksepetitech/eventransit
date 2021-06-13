using AutoMapper;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<SaveEventDto, Event>()
                .ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.EventName));
        }
    }
}