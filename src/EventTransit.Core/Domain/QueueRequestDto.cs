namespace EventTransit.Core.Domain
{
    public class QueueRequestDto
    {
        public string Name { get; set; }

        public dynamic Payload { get; set; }
    }
}