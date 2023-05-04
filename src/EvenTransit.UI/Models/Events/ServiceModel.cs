
namespace EvenTransit.UI.Models.Events;

public class ServiceModel
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int Timeout { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> CustomBodyMap { get; set; }
}
