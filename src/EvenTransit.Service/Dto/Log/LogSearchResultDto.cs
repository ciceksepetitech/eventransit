using EvenTransit.Domain.Enums;

namespace EvenTransit.Service.Dto.Log;

public class LogSearchResultDto
{
    public List<LogFilterItemDto> Items { get; set; }
    public int TotalPages { get; set; }
}

public class LogFilterItemDto
{
    public string Id { get; set; }
    public string EventName { get; set; }
    public string ServiceName { get; set; }
    public string CorrelationId { get; set; }
    public LogType LogType { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ConsumeDate { get; set; }
    public long? Retry { get; set; }
}
