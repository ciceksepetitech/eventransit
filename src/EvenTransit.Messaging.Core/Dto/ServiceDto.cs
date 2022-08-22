namespace EvenTransit.Messaging.Core.Dto;

public record ServiceDto
{
    public string Name { get; init; }
    public string Url { get; init; }
    public int Timeout { get; init; }
    public string Method { get; init; }
    public Dictionary<string, string> Headers { get; init; }
    public Dictionary<string, string> CustomBodyMap { get; init; }

    public ServiceDto()
    {
    }
}
