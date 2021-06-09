using AutoMapper;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.Core.Entities;

namespace EvenTransit.Service.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<Logs, LogFilterItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id));
        }
    }
}