namespace EvenTransit.Api.Models
{
    public class EventRequest
    {
        public string EventName { get; set; }

        public object Payload { get; set; }
    }
}