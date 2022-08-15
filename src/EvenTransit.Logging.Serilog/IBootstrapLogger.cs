namespace EvenTransit.Logging.Serilog;

public interface IBootstrapLogger: IDisposable
{
    void Configure<TConfiguration>(Action<TConfiguration>? config);
    void LogInformation(string message);
    void LogError(Exception ex, string message);
}
