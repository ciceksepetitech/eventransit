using System;
using EvenTransit.Domain.Enums;

namespace EvenTransit.Service.Dto.Log;

public class LogSearchRequestDto
{
    public DateTime? LogDateFrom { get; set; }
    public DateTime? LogDateTo { get; set; }
    public LogType LogType { get; set; }
    public string EventName { get; set; }
    public string ServiceName { get; set; }
    public int Page { get; set; }
    public string RequestBodyRegex { get; set; }
}
