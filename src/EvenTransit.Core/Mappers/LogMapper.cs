using AutoMapper;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<EventLogDto, LogsDto>();
            CreateMap<EventDetailDto, LogDetailDto>();
            CreateMap<HttpRequestDto, LogDetailRequestDto>();
            CreateMap<HttpResponseDto, LogDetailResponseDto>();
        }
    }
}