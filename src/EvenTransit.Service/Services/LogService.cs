using AutoMapper;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Service.Abstractions;
using EvenTransit.Service.Dto.Log;
using EvenTransit.Service.Rules.Log;
using System.Linq.Expressions;

namespace EvenTransit.Service.Services;

public class LogService : ILogService
{
    private readonly ILogsRepository _logsRepository;
    private readonly ILogStatisticsRepository _logStatisticsRepository;
    private readonly IMapper _mapper;
    private readonly IEventsRepository _eventsRepository;
    private readonly IEventLogStatisticRepository _eventLogStatisticRepository;

    public LogService(ILogsRepository logsRepository,
        IMapper mapper,
        ILogStatisticsRepository logStatisticsRepository,
        IEventsRepository eventsRepository,
        IEventLogStatisticRepository eventLogStatisticRepository)
    {
        _logsRepository = logsRepository;
        _mapper = mapper;
        _logStatisticsRepository = logStatisticsRepository;
        _eventsRepository = eventsRepository;
        _eventLogStatisticRepository = eventLogStatisticRepository;
    }

    public async Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request)
    {
        var argParam = Expression.Parameter(typeof(Logs), "x");

        var eventNamePredicateHandler = new EventNamePredicateHandler();
        var serviceNamePredicateHandler = new ServiceNamePredicateHandler();
        var logTypePredicateHandler = new LogTypePredicateHandler();
        var dateFromPredicateHandler = new DateFromPredicateHandler();
        var dateToPredicateHandler = new DateToPredicateHandler();
        // var queryPredictionHandler = new QueryPredictionHandler();

        eventNamePredicateHandler.SetSuccessor(serviceNamePredicateHandler);
        serviceNamePredicateHandler.SetSuccessor(logTypePredicateHandler);
        logTypePredicateHandler.SetSuccessor(dateFromPredicateHandler);
        dateFromPredicateHandler.SetSuccessor(dateToPredicateHandler);
        // dateFromPredicateHandler.SetSuccessor(queryPredictionHandler);

        var expression = eventNamePredicateHandler.HandleRequest(argParam, Expression.Constant(true), request);
        var lambda = Expression.Lambda<Func<Logs, bool>>(expression, argParam);
        var result = await _logsRepository.GetLogsAsync(lambda, request.RequestBodyRegex, request.Page);

        return new LogSearchResultDto
        {
            Items = _mapper.Map<List<LogFilterItemDto>>(result.Items),
            TotalPages = result.TotalPages
        };
    }

    public async Task<LogSearchResultDto> SearchAsync(string correlationId)
    {
        var result = await _logsRepository.GetLogsAsync(x => x.Details.CorrelationId == correlationId, null, 1);

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
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var logStatistics = await _logStatisticsRepository.GetStatisticsAsync(startDate, endDate);

        var dates = logStatistics.Select(x => x.Date.ToString("dd-MM-yyyy")).ToList();
        var successCount = logStatistics.Select(x => x.SuccessCount).ToList();
        var failCount = logStatistics.Select(x => x.FailCount).ToList();

        response.Dates = dates;
        response.SuccessCount = successCount;
        response.FailCount = failCount;

        return response;
    }

    public async Task RefreshEventLogStatistic()
    {
        var events = await _eventsRepository.GetEventsAsync();
        foreach (var eventDto in events)
        {
            var logCount = _logsRepository.GetLogsCountByEvent(eventDto.Name, DateTime.Now.AddDays(-5));

            var eventLogData = await _eventLogStatisticRepository.GetAsync(eventDto.Id);
            if(eventLogData != null)
            {
                eventLogData.SuccessCount = logCount.Item1;
                eventLogData.FailCount = logCount.Item2;

                await _eventLogStatisticRepository.UpdateAsync(eventLogData.EventId, eventLogData);

                Console.WriteLine(eventDto.Name);
            }
        }
    }
}
