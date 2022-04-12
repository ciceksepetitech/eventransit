using System;
using EvenTransit.Domain.Enums;

namespace EvenTransit.Messaging.RabbitMq;

public static class Extensions
{
    private const string RetryExchangeSuffix = "retry";
    private const string RetryQueueSuffix = "retry";

    public static string GetRetryExchangeName(this string exchangeName)
    {
        return $"{exchangeName}.{RetryExchangeSuffix}";
    }

    public static string GetRetryQueueName(this string queueName, string eventName, RetryTimes retryTime)
    {
        return $"{eventName}.{queueName}.{RetryQueueSuffix}.{Enum.GetName(typeof(RetryTimes), retryTime)?.ToLower()}";
    }

    public static string GetQueueName(this string queueName, string eventName)
    {
        return $"{eventName}.{queueName}";
    }
}
