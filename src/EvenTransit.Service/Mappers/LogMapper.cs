using AutoMapper;
using EvenTransit.Data.Entities;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<Logs, LogItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id));

            CreateMap<LogDetail, LogItemDetailDto>();
            CreateMap<LogDetailRequest, LogItemDetailRequestDto>();
            CreateMap<LogDetailResponse, LogItemDetailResponseDto>();

            CreateMap<Logs, LogFilterItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src._id));
        }
    }
}