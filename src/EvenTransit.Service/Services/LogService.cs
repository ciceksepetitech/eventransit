using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Core.Abstractions.Data.DataServices;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto;
using EvenTransit.Core.Dto.Service.Log;
using EvenTransit.Core.Entities;
using EvenTransit.Service.Rules.Log;

namespace EvenTransit.Service.Services
{
    public class LogService : ILogService
    {
        private readonly ILogsDataService _logsDataService;
        private readonly IMapper _mapper;

        public LogService(ILogsDataService logsDataService, IMapper mapper)
        {
            _logsDataService = logsDataService;
            _mapper = mapper;
        }

        public async Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request)
        {
            var argParam = Expression.Parameter(typeof(Logs), "x");

            var eventNamePredicateHandler = new EventNamePredicateHandler();
            var serviceNamePredicateHandler = new ServiceNamePredicateHandler();
            var logTypePredicateHandler = new LogTypePredicateHandler();
            var dateFromPredicateHandler = new DateFromPredicateHandler();
            var dateToPredicateHandler = new DateToPredicateHandler();

            eventNamePredicateHandler.SetSuccessor(serviceNamePredicateHandler);
            serviceNamePredicateHandler.SetSuccessor(logTypePredicateHandler);
            logTypePredicateHandler.SetSuccessor(dateFromPredicateHandler);
            dateFromPredicateHandler.SetSuccessor(dateToPredicateHandler);

            var expression = eventNamePredicateHandler.HandleRequest(argParam, Expression.Constant(true), request);
            var lambda = Expression.Lambda<Func<Logs, bool>>(expression, argParam);
            var result = await _logsDataService.GetLogsAsync(lambda, request.Page);

            return new LogSearchResultDto
            {
                Items = _mapper.Map<List<LogFilterItemDto>>(result.Items),
                TotalPages = result.TotalPages
            };
        }

        public async Task<LogsDto> GetByIdAsync(string id)
        {
            return await _logsDataService.GetByIdAsync(id);
        }

        public async Task<LogStatisticsDto> GetDashboardStatistics()
        {
            var response = new LogStatisticsDto();
            var currentTime = DateTime.UtcNow;

            for (var i = -4; i <= 0; i++)
            {
                var day = currentTime.AddDays(i);
                var logsCount = await _logsDataService.GetLogsCountByDay(day);
                
                response.Dates.Add(day.ToString("yyyy-MM-dd"));
                response.SuccessCount.Add(logsCount.Item1);
                response.FailCount.Add(logsCount.Item2);
            }
            
            return response;
        }
    }
}