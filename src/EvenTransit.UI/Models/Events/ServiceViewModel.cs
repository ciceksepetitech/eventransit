
namespace EvenTransit.UI.Models.Events;

public class ServiceViewModel
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Method { get; set; }
    public int Timeout { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public long SuccessCount { get; set; }
    public long FailCount { get; set; }
}
