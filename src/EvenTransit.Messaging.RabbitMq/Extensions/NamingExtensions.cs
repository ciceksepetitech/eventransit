namespace EvenTransit.Messaging.RabbitMq.Extensions;

public static class NamingExtensions
{
    private const string RetryExchangeSuffix = "retry";
    private const string RetryQueueSuffix = "retry";
    private const string DelayExchangeSuffix = "delay";
    private const string DelayQueueSuffix = "delay";

    public static string GetRetryExchangeName(this string exchangeName)
    {
        return $"{exchangeName}.{RetryExchangeSuffix}";
    }

    public static string GetRetryQueueName(this string queueName, string eventName)
    {
        return $"{eventName}.{queueName}.{RetryQueueSuffix}";
    }
    
    public static string GetDelayExchangeName(this string exchangeName)
    {
        return $"{exchangeName}.{DelayExchangeSuffix}";
    }

    public static string GetDelayQueueName(this string queueName, string eventName)
    {
        return $"{eventName}.{queueName}.{DelayQueueSuffix}";
    }

    public static string GetQueueName(this string queueName, string eventName)
    {
        return $"{eventName}.{queueName}";
    }
}
