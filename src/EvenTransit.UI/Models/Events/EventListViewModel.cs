using System;

namespace EvenTransit.UI.Models.Events;

public class EventListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int ServiceCount { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
}
