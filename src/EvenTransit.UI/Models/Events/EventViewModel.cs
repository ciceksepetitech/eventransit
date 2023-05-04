
namespace EvenTransit.UI.Models.Events;

public class EventViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ServiceViewModel> Services { get; set; }
}
