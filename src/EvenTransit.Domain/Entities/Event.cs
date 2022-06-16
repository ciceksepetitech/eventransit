namespace EvenTransit.Domain.Entities;

public class Event : BaseEntity
{
    public Event()
    {
        Services = new List<Service>();
    }

    public string Name { get; set; }
    public List<Service> Services { get; set; }
}
