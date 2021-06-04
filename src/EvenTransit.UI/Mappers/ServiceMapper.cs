using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.Core.Dto.Service.Event;
using EvenTransit.UI.Models.Events;

namespace EvenTransit.UI.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<ServiceDto, ServiceModel>();
            CreateMap<SaveServiceModel, SaveServiceDto>();
        }
    }
}