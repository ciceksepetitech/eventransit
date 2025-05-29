using EvenTransit.Domain.Enums;

namespace EvenTransit.UI.Models.Logs;

public class LogSearchResultViewModel
{
    public string Id { get; set; }
    public string EventName { get; set; }
    public string ServiceName { get; set; }
    public string CorrelationId { get; set; }
    public LogType LogType { get; set; }
    public string CreatedOnString { get; set; }
    public string PublishDateString { get; set; }
    public string ConsumeDateString { get; set; }
    public long? Retry { get; set; }
}
