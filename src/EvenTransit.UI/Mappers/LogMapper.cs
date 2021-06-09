using AutoMapper;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.UI.Models.Logs;

namespace EvenTransit.UI.Mappers
{
    public class LogMapper : Profile
    {
        public LogMapper()
        {
            CreateMap<LogFilterModel, LogSearchRequestDto>();
            CreateMap<LogFilterItemDto, LogSearchResultViewModel>();

            CreateMap<LogsDto, LogItemViewModel>();
            CreateMap<LogDetailDto, LogItemDetailViewModel>();
            CreateMap<LogDetailRequestDto, HttpRequestViewModel>();
            CreateMap<LogDetailResponseDto, HttpResponseViewModel>();
        }
    }
}