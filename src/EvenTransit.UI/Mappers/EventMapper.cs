using AutoMapper;
using EvenTransit.Service.Dto.Event;
using EvenTransit.UI.Models.Events;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvenTransit.UI.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<EventDto, EventViewModel>();
            CreateMap<EventDto, EventListViewModel>();
            CreateMap<ServiceDto, ServiceViewModel>();
            CreateMap<EventDto, SelectListItem>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name));
            CreateMap<SaveEventModel, SaveEventDto>();
        }
    }
}