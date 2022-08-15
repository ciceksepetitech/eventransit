using EvenTransit.Domain.Enums;

namespace EvenTransit.Domain.Configuration;

public class EvenTransitConfig
{
    public EvenTransitLoggingConfig Logging { get; set; }
    public Mode Mode { get; set; }
}

public class EvenTransitLoggingConfig
{
    public List<string> Cookies { get; set; } = new();
    public List<string> Headers { get; set; } = new();
}
