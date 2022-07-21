﻿using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Messaging.Core.Abstractions;

namespace EvenTransit.Messaging.Core.Domain;

public class EventLog : IEventLog
{
    private readonly ILogsRepository _logsRepository;

    public EventLog(ILogsRepository logsRepository)
    {
        _logsRepository = logsRepository;
    }

    public async Task LogAsync(Logs details)
    {
        await _logsRepository.InsertLogAsync(details);
    }
}
