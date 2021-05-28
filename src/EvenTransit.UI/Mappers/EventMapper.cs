using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.UI.Models;

namespace EvenTransit.UI.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<EventDto, Core.Dto.UI.EventDto>();
            CreateMap<ServiceDto, Core.Dto.UI.ServiceDto>();
            
            // TODO Separate API and Service mapper profiles
            CreateMap<SaveServiceModel, SaveServiceDto>();
        }
    }
}