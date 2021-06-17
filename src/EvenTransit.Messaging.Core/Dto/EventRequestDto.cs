namespace EvenTransit.Messaging.Core.Dto
{
    public class EventRequestDto
    {
        public string EventName { get; set; }

        public dynamic Payload { get; set; }
    }
}