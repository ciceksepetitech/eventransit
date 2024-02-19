using EvenTransit.Domain.Enums;

namespace EvenTransit.Service.Dto.Log;

public class LogItemDto
{
    public string Id { get; set; }

    public string EventName { get; set; }

    public string ServiceName { get; set; }

    public LogType LogType { get; set; }

    public DateTime CreatedOn { get; set; }

    public LogItemDetailDto Details { get; set; }
}

public class LogItemDetailDto
{
    public LogItemDetailRequestDto Request { get; set; }

    public LogItemDetailResponseDto Response { get; set; }

    public string Message { get; set; }
}

public class LogItemDetailRequestDto
{
    public string Url { get; set; }
    public int Timeout { get; set; }
    public string Method { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}

public class LogItemDetailResponseDto
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Response { get; set; }
}
