namespace EvenTransit.Core.Constants
{
    public static class CacheConstants
    {
        public const string RedisConnectionStringKey = "REDIS_CS";
        public const string QueuesByEventKey = "Queues_{0}";
        public const string EventsKey = "Events";
    }
}