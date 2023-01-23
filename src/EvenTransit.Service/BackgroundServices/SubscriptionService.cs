using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Service.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Service.BackgroundServices
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IEnumerable<IConsumer<T>> GetSubscriptions<T>() where T : IInternalEvent
        {
            return _serviceProvider.GetServices<IConsumer<T>>();
        }

        /// <summary>
        /// Get async subscriptions (IAsyncConsumer)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IEnumerable<IAsyncConsumer<T>> GetAsyncSubscriptions<T>() where T : IInternalAsyncEvent
        {
            return _serviceProvider.GetServices<IAsyncConsumer<T>>();
        }
    }
}
