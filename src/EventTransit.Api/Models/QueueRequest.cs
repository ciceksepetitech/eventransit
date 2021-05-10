namespace EventTransit.Api.Models
{
    public class QueueRequest
    {
        public string Name { get; set; }

        public dynamic Payload { get; set; }
    }
}