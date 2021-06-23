namespace EvenTransit.Messaging.RabbitMq
{
    public static class Extensions
    {
        private const string RetryExchangeSuffix = "retry";
        private const string RetryQueueSuffix = "retry";

        public static string GetRetryExchangeName(this string exchangeName) => $"{exchangeName}.{RetryExchangeSuffix}";
        public static string GetRetryQueueName(this string queueName) => $"{queueName}.{RetryQueueSuffix}";
    }
}