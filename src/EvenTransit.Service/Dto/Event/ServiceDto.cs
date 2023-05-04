
namespace EvenTransit.Service.Dto.Event;

public class ServiceDto
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int Timeout { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> CustomBodyMap { get; set; }
}
