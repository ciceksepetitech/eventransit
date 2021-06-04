using AutoMapper;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.Core.Entities;
using EvenTransit.UI.Models.Logs;

namespace EvenTransit.UI.Mappers
{
    public class LogService : Profile
    {
        public LogService()
        {
            CreateMap<LogFilterModel, LogSearchRequestDto>();
            CreateMap<LogFilterItemDto, LogSearchResultViewModel>();

            CreateMap<Logs, LogFilterItemDto>();
        }
    }
}