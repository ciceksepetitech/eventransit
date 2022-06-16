namespace EvenTransit.UI.Models.Events;

public class SaveServiceModel
{
    public Guid EventId { get; set; }
    public string ServiceName { get; set; }
    public string HiddenServiceName { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public int Timeout { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> CustomBodyMap { get; set; }
}
