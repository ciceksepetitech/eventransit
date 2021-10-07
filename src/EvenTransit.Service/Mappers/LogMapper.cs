using AutoMapper;
using EvenTransit.Domain.Entities;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<Logs, LogItemDto>();

            CreateMap<LogDetail, LogItemDetailDto>();
            CreateMap<LogDetailRequest, LogItemDetailRequestDto>();
            CreateMap<LogDetailResponse, LogItemDetailResponseDto>();

            CreateMap<Logs, LogFilterItemDto>()
                .ForMember(dest => dest.CorrelationId, cfg => cfg.MapFrom(src => src.Details.CorrelationId));
        }
    }
}