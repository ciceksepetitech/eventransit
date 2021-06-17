using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Data.Abstractions;
using EvenTransit.Data.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.Service.Rules.Log;

namespace EvenTransit.Service.Services
{
    public class LogService : ILogService
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IMapper _mapper;

        public LogService(ILogsRepository logsRepository, IMapper mapper)
        {
            _logsRepository = logsRepository;
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
            var result = await _logsRepository.GetLogsAsync(lambda, request.Page);

            return new LogSearchResultDto
            {
                Items = _mapper.Map<List<LogFilterItemDto>>(result.Items),
                TotalPages = result.TotalPages
            };
        }

        public async Task<LogItemDto> GetByIdAsync(string id)
        {
            var data = await _logsRepository.GetByIdAsync(id);
            return _mapper.Map<LogItemDto>(data);
        }

        public async Task<LogStatisticsDto> GetDashboardStatistics()
        {
            var response = new LogStatisticsDto();
            var currentTime = DateTime.UtcNow;

            for (var i = -4; i <= 0; i++)
            {
                var day = currentTime.AddDays(i);
                var logsCount = await GetLogsCountByDay(day);
                
                response.Dates.Add(day.ToString("yyyy-MM-dd"));
                response.SuccessCount.Add(logsCount.Item1);
                response.FailCount.Add(logsCount.Item2);
            }
            
            return response;
        }
        
        private async Task<(long, long)> GetLogsCountByDay(DateTime day)
        {
            var startDate = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            var endDate = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

            var successLogsCount = await _logsRepository.GetLogsCount(startDate, endDate, LogType.Success);
            var failLogsCount = await _logsRepository.GetLogsCount(startDate, endDate, LogType.Fail);

            return (successLogsCount, failLogsCount);
        }
    }
}