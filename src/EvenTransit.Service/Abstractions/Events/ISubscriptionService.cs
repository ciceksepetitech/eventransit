using EvenTransit.Messaging.Core.Abstractions;

namespace EvenTransit.Service.Abstractions.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        IEnumerable<IConsumer<T>> GetSubscriptions<T>() where T : IInternalEvent;

        /// <summary>
        /// Get async subscriptions (IAsyncConsumer)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        IEnumerable<IAsyncConsumer<T>> GetAsyncSubscriptions<T>() where T : IInternalAsyncEvent;
    }
}
