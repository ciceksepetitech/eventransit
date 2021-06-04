using System.Collections.Generic;

namespace EvenTransit.Core.Dto.Service.Event
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ServiceDto> Services { get; set; }
    }
}