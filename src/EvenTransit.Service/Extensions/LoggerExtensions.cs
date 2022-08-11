using Microsoft.Extensions.Logging;

namespace EvenTransit.Service.Extensions;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> EventApiOperationFailedAction;
    private static readonly Action<ILogger, string, Exception> ConsumerBinderFailedAction;

    static LoggerExtensions()
    {
        EventApiOperationFailedAction = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(EventApiOperationFailed)),
            "Event API operation failed. {Message}");
        
        ConsumerBinderFailedAction = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(2, nameof(EventApiOperationFailed)),
            "Consumer binder failed. {Message}");
    }
    
    public static void EventApiOperationFailed(this ILogger logger, Exception e, string message)
    {
        EventApiOperationFailedAction(logger, message, e);
    }
    
    public static void ConsumerBinderFailed(this ILogger logger, Exception e, string message)
    {
        ConsumerBinderFailedAction(logger, message, e);
    }
}