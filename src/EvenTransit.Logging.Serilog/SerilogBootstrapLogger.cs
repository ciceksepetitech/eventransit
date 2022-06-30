using EvenTransit.Logging.Serilog.Formatting;
using EvenTransit.Logging.Serilog.Sanitazing;
using Serilog;

namespace EvenTransit.Logging.Serilog;

public class SerilogBootstrapLogger : IBootstrapLogger
{
    public static readonly HttpEnricher HttpEnricher = new();

    public void LogInformation(string message)
    {
        Log.Information("Starting web host...");
    }

    public void LogError(Exception ex, string message)
    {
        Log.Error(ex, "Host terminated unexpectedly!");
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }

    public void Configure<TConfiguration>(Action<TConfiguration> config)
    {
        var configuration = new LoggerConfiguration()
            .SetDefaultLogLevel("LOG_LEVEL")
            .MinimumLevel.Override("Microsoft", "LOG_LEVEL_MICROSOFT".GetLogLevel())
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", "LOG_LEVEL_MICROSOFT_HOSTING_LIFETIME".GetLogLevel())
            .MinimumLevel.Override("System", "LOG_LEVEL_SYSTEM".GetLogLevel())
            .Enrich.With(HttpEnricher);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "development")
            configuration.WriteTo.Console();
        else
            configuration.WriteTo.Console(new SanitizerFormatter(new List<ISanitizingFormatRule>
            {
                new EmailSanitizingFormatRule(),
                new CreditCardSanitizingFormatRule(true),
                new PasswordSanitizingFormatRule()
            }, new RenderedCompactJsonFormatter()));

        (config as Action<LoggerConfiguration>)?.Invoke(configuration);

        Log.Logger = configuration.CreateLogger();
    }
}
