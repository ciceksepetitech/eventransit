using EvenTransit.Domain.Enums;

namespace EvenTransit.Messaging.Core.Dto;

public class EventLogDto
{
    public string EventName { get; set; }
    public string ServiceName { get; set; }
    public LogType LogType { get; set; }
    public EventDetailDto Details { get; set; }
}

public class EventDetailDto
{
    public HttpRequestDto Request { get; set; }
    public EventLogHttpResponseDto Response { get; set; }
    public string Message { get; set; }
    public string CorrelationId { get; set; }
    public DateTime? PublishDate { get; set; }
    public long Retry { get; set; }
}
