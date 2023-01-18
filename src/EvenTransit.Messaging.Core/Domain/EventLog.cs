using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.InternalEvents;

namespace EvenTransit.Messaging.Core.Domain;

public class EventLog : IEventLog
{
    private readonly ILogsRepository _logsRepository;
    private readonly IInternalEventPublisher _internalEventPublisher;

    public EventLog
    (
        ILogsRepository logsRepository,
        IInternalEventPublisher internalEventPublisher
    )
    {
        _logsRepository = logsRepository;
        _internalEventPublisher = internalEventPublisher;
    }

    public async Task LogAsync(Logs details)
    {
        await _logsRepository.InsertLogAsync(details);
        await _internalEventPublisher.FireAndForgetAsync(new StatisticsUpdatedEvent { Details = details });
    }
}
