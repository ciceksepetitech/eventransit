namespace EvenTransit.Messaging.Core.Dto;

public class HttpRequestDto
{
    public string Url { get; set; }
    public int Timeout { get; set; }
    public object Body { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> Fields { get; set; }
}
