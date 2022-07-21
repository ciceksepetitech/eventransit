using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.Service.Rules.Log;
using System.Text.Json;

namespace EvenTransit.Service.Services;

public class LogService : ILogService
{
    private readonly ILogsRepository _logsRepository;
    private readonly ILogStatisticsRepository _logStatisticsRepository;
    private readonly IMapper _mapper;
    private readonly IHttpProcessor _httpProcessor;

    public LogService(ILogsRepository logsRepository,
        IMapper mapper,
        ILogStatisticsRepository logStatisticsRepository,
        IHttpProcessor httpProcessor)
    {
        _logsRepository = logsRepository;
        _mapper = mapper;
        _logStatisticsRepository = logStatisticsRepository;
        _httpProcessor = httpProcessor;
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

    public async Task<LogSearchResultDto> SearchAsync(string correlationId)
    {
        var result = await _logsRepository.GetLogsAsync(x => x.Details.CorrelationId == correlationId, 1);

        return new LogSearchResultDto
        {
            Items = _mapper.Map<List<LogFilterItemDto>>(result.Items),
            TotalPages = result.TotalPages
        };
    }

    public async Task<LogItemDto> GetByIdAsync(Guid id)
    {
        var data = await _logsRepository.GetByIdAsync(id);
        return _mapper.Map<LogItemDto>(data);
    }

    public async Task<LogStatisticsDto> GetDashboardStatistics()
    {
        var response = new LogStatisticsDto();
        var startDate = DateTime.Today.AddDays(-4);
        var endDate = DateTime.UtcNow;
        var logStatistics = await _logStatisticsRepository.GetStatisticsAsync(startDate, endDate);

        var dates = logStatistics.Select(x => x.Date.ToString("yyyy-MM-dd")).ToList();
        var successCount = logStatistics.Select(x => x.SuccessCount).ToList();
        var failCount = logStatistics.Select(x => x.FailCount).ToList();

        response.Dates = dates;
        response.SuccessCount = successCount;
        response.FailCount = failCount;

        return response;
    }

    public async Task<bool> ResendRequest(LogItemDto data)
    {
        if (string.IsNullOrEmpty(data.Details.Request.Method))
            return false;

        var serviceDto = new ServiceDto()
        {
            Name = data.ServiceName,
            Url = data.Details.Request.Url,
            Timeout = data.Details.Request.Timeout,
            Method = data.Details.Request.Method,
            Headers = data.Details.Request.Headers
        };

        var eventPublishDto = new EventPublishDto()
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Payload = JsonSerializer.Deserialize<object>(data.Details.Request.Body)
        };

        var result = await _httpProcessor.ProcessAsync(data.EventName, serviceDto, eventPublishDto);

        return result;
    }

    private async Task<(long, long)> GetLogsCountByDay(DateTime day)
    {
        var startDate = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
        var endDate = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

        var successLogsCount = await _logsRepository.GetLogsCountAsync(startDate, endDate, LogType.Success);
        var failLogsCount = await _logsRepository.GetLogsCountAsync(startDate, endDate, LogType.Fail);

        return (successLogsCount, failLogsCount);
    }
}
