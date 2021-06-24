namespace EvenTransit.Messaging.Core
{
    public static class MessagingConstants
    {
        public const string NewServiceExchange = "NewService";
        public const int DeadLetterQueueTTL = 5000;
        public const string RetryHeaderName = "x-retry-count";
        public const int MaxRetryCount = 5;
    }
}