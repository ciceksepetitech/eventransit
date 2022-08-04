namespace EvenTransit.Messaging.Core.Dto;

public record ServiceDto(string Name, string Url, int Timeout, string Method,
    Dictionary<string, string> Headers, Dictionary<string, string> CustomBodyMap);
