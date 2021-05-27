using System.Collections.Generic;

namespace EvenTransit.Core.Dto.UI
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ServiceDto> Services { get; set; }
    }
}