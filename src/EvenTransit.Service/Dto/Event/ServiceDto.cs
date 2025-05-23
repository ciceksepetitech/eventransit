﻿namespace EvenTransit.Service.Dto.Event;

public class ServiceDto
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int Timeout { get; set; }
    public int DelaySeconds { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> CustomBodyMap { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
}
