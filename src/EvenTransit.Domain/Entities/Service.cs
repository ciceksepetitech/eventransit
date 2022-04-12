using System.Collections.Generic;

namespace EvenTransit.Domain.Entities;

public class Service
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int Timeout { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}
