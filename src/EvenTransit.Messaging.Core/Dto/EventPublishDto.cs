namespace EvenTransit.Messaging.Core.Dto;

public class EventPublishDto
{
    public object Payload { get; set; }
    public string CorrelationId { get; set; }
    public DateTime? PublishDate { get; set; }
    public Dictionary<string, string> Fields { get; set; }
}
