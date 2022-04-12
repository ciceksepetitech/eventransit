using AutoMapper;
using EvenTransit.Service.Dto.Log;
using EvenTransit.UI.Models.Logs;

namespace EvenTransit.UI.Mappers;

public class LogMapper : Profile
{
    public LogMapper()
    {
        CreateMap<LogFilterModel, LogSearchRequestDto>();
        CreateMap<LogFilterItemDto, LogSearchResultViewModel>();

        CreateMap<LogItemDto, LogItemViewModel>();
        CreateMap<LogItemDetailDto, LogItemDetailViewModel>();
        CreateMap<LogItemDetailRequestDto, HttpRequestViewModel>();
        CreateMap<LogItemDetailResponseDto, HttpResponseViewModel>();
    }
}
