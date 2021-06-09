using AutoMapper;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Entities;

namespace EvenTransit.Data.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<LogsDto, Logs>().ReverseMap();
            CreateMap<LogDetailDto, LogDetail>().ReverseMap();
            CreateMap<LogDetailRequestDto, LogDetailRequest>().ReverseMap();
            CreateMap<LogDetailResponseDto, LogDetailResponse>().ReverseMap();
        }
    }
}