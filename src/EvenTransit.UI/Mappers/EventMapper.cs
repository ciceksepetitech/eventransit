using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.UI.Models.Events;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvenTransit.UI.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<EventDto, Core.Dto.UI.EventDto>();
            CreateMap<ServiceDto, Core.Dto.UI.ServiceDto>();
            CreateMap<EventDto, SelectListItem>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name));
        }
    }
}