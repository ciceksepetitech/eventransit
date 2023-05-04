
namespace EvenTransit.Service.Dto.Event;

public class SaveServiceDto
{
    public Guid EventId { get; set; }
    public string ServiceName { get; set; }
    public string HiddenServiceName { get; set; }
    public string Url { get; set; }
    public int Timeout { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> CustomBodyMap { get; set; }
}
