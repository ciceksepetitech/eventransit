using AutoMapper;
using EvenTransit.Core.Dto.Service;
using EvenTransit.UI.Models;

namespace EvenTransit.UI.Mappers
{
    public class ServiceMapper : Profile
    {
        public ServiceMapper()
        {
            CreateMap<ServiceDto, ServiceModel>();
        }
    }
}