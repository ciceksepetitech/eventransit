using AutoMapper;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<LogsDto, Logs>();
            CreateMap<LogDetailDto, LogDetail>();
            CreateMap<LogDetailRequestDto, LogDetailRequest>();
            CreateMap<LogDetailResponseDto, LogDetailResponse>();
        }
    }
}