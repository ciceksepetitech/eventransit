using Serilog;
using Serilog.Events;

namespace EvenTransit.Logging.Serilog;

internal static class SerilogLogLevelExtensions
{
    internal static LoggerConfiguration SetDefaultLogLevel(this LoggerConfiguration loggerConfiguration, string key)
    {
        var defaultLogLevel = GetLogLevel(key);
        switch (defaultLogLevel)
        {
            case LogEventLevel.Verbose:
                loggerConfiguration.MinimumLevel.Verbose();
                break;
            case LogEventLevel.Debug:
                loggerConfiguration.MinimumLevel.Debug();
                break;
            case LogEventLevel.Information:
                loggerConfiguration.MinimumLevel.Information();
                break;
            case LogEventLevel.Warning:
                loggerConfiguration.MinimumLevel.Warning();
                break;
            case LogEventLevel.Error:
                loggerConfiguration.MinimumLevel.Error();
                break;
            case LogEventLevel.Fatal:
                loggerConfiguration.MinimumLevel.Fatal();
                break;
            default:
                loggerConfiguration.MinimumLevel.Information();
                break;
        }

        return loggerConfiguration;
    }

    internal static LogEventLevel GetLogLevel(this string key)
    {
        return Environment.GetEnvironmentVariable(key) switch
        {
            "Trace" => LogEventLevel.Verbose,
            "Debug" => LogEventLevel.Debug,
            "Information" => LogEventLevel.Information,
            "Warning" => LogEventLevel.Warning,
            "Error" => LogEventLevel.Error,
            "Critical" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
