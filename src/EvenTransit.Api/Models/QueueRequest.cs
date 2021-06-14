namespace EvenTransit.Api.Models
{
    public class QueueRequest
    {
        public string EventName { get; set; }

        public dynamic Payload { get; set; }
    }
}