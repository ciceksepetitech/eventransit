using EvenTransit.Domain.Enums;
using System.Globalization;

namespace EvenTransit.UI.Models.Logs;

public class LogItemViewModel
{
    public string Id { get; set; }

    public string EventName { get; set; }

    public string ServiceName { get; set; }

    public LogType LogType { get; set; }

    public DateTime CreatedOn { get; set; }

    public string TotalDuration { get => Details.PublishDate.HasValue ? ((long)(CreatedOn - Details.PublishDate.Value).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) : ""; }

    public string ConsumeDuration { get => Details.ConsumeDate.HasValue ? ((long)(CreatedOn - Details.ConsumeDate.Value).TotalMilliseconds).ToString(CultureInfo.InvariantCulture) : ""; }

    public LogItemDetailViewModel Details { get; set; }
}

public class LogItemDetailViewModel
{
    public HttpRequestViewModel Request { get; set; }

    public HttpResponseViewModel Response { get; set; }

    public string Message { get; set; }

    public string CorrelationId { get; set; }

    public DateTime? PublishDate { get; set; }

    public DateTime? ConsumeDate { get; set; }

    public long? Retry { get; set; }
}
