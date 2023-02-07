using AutoMapper;
using EvenTransit.Domain.Extensions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.UI.Models.Logs;

namespace EvenTransit.UI.Mappers;

public class LogMapper : Profile
{
    public LogMapper()
    {
        CreateMap<LogFilterModel, LogSearchRequestDto>()
            .ForMember(s => s.RequestBodyRegex, m => m.MapFrom(s => s.Query));

        CreateMap<LogFilterItemDto, LogSearchResultViewModel>()
            .ForMember(s => s.CreatedOnString, m => m.MapFrom(s => s.CreatedOn.ConvertToLocalDateString()));

        CreateMap<LogItemDto, LogItemViewModel>();
        CreateMap<LogItemDetailDto, LogItemDetailViewModel>();
        CreateMap<LogItemDetailRequestDto, HttpRequestViewModel>();
        CreateMap<LogItemDetailResponseDto, HttpResponseViewModel>();
    }
}
