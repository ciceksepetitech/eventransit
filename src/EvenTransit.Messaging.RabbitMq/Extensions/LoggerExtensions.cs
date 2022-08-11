using Microsoft.Extensions.Logging;

namespace EvenTransit.Messaging.RabbitMq.Extensions;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception> ChannelStateAction;
    private static readonly Action<ILogger, string, object, Exception> ChannelStateFailedAction;
    private static readonly Action<ILogger, string, Exception> ConnectionStateFailedAction;
    private static readonly Action<ILogger, string, Exception> ConsumerFailedAction;

    static LoggerExtensions()
    {
        ChannelStateAction = LoggerMessage.Define<string>(
            LogLevel.Information, 
            new EventId(101, nameof(ChannelState)), 
            "{Message}");
        
        ChannelStateFailedAction = LoggerMessage.Define<string, object>(
            LogLevel.Error,
            new EventId(102, nameof(ChannelStateFailed)),
            "Channel failed Message = {Message}, Cause = '{Cause}'");
        
        ConnectionStateFailedAction = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(103, nameof(ConnectionStateFailed)),
            "Connection failed Message = {Message}");
        
        ConsumerFailedAction = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(104, nameof(ConsumerFailedAction)),
            "Consumer failed Message = {Message}");
    }
    
    public static void ChannelState(this ILogger logger, string message)
    {
        ChannelStateAction(logger, message, null);
    }
    
    public static void ChannelStateFailed(this ILogger logger, string message, object cause, Exception e)
    {
        ChannelStateFailedAction(logger, message, cause, e);
    }
    
    public static void ConnectionStateFailed(this ILogger logger, string message, Exception e)
    {
        ConnectionStateFailedAction(logger, message, e);
    }
    
    public static void ConsumerFailed(this ILogger logger, string message, Exception e)
    {
        ConsumerFailedAction(logger, message, e);
    }
}