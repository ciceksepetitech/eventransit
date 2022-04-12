namespace EvenTransit.Messaging.Core.Dto;

public class BaseHttpResponseDto
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Response { get; set; }
}

public class HttpResponseDto : BaseHttpResponseDto
{
}

public class EventLogHttpResponseDto : BaseHttpResponseDto
{
}
