using System.Collections.Generic;

namespace EvenTransit.Service.Dto.Event;

public class EventDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int ServiceCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
    public List<ServiceDto> Services { get; set; }
}
