using AutoMapper;

namespace EvenTransit.UI.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<EvenTransit.Core.Dto.Service.EventDto, Core.Dto.UI.EventDto>();
            CreateMap<EvenTransit.Core.Dto.Service.ServiceDto, Core.Dto.UI.ServiceDto>();
        }
    }
}