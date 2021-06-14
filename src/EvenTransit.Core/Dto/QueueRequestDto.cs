namespace EvenTransit.Core.Dto
{
    public class QueueRequestDto
    {
        public string EventName { get; set; }

        public dynamic Payload { get; set; }
    }
}