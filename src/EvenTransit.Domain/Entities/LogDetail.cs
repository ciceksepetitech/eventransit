namespace EvenTransit.Domain.Entities;

public class LogDetail
{
    public string Message { get; set; }
    public LogDetailRequest Request { get; set; }
    public LogDetailResponse Response { get; set; }
    public string CorrelationId { get; set; }
}

public class LogDetailRequest
{
    public string Url { get; set; }
    public int Timeout { get; set; }
    public int DelaySeconds { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}

public class LogDetailResponse
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Response { get; set; }
}
