namespace EvenTransit.UI.Models.Api;

public class EventRequest
{
    public string EventName { get; set; }

    public object Payload { get; set; }

    public Dictionary<string, string> Fields { get; set; }
}
