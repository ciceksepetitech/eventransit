namespace EvenTransit.Domain.Configuration;

public class EvenTransitConfig
{
    public EvenTransitLoggingConfig Logging { get; set; }
}

public class EvenTransitLoggingConfig
{
    public List<string> Cookies { get; set; } = new List<string>();
    public List<string> Headers { get; set; } = new List<string>();
}
