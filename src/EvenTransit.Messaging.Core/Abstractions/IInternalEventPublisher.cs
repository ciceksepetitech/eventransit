namespace EvenTransit.Messaging.Core.Abstractions
{
    /// <summary>
    /// Event publisher
    /// </summary>
    public interface IInternalEventPublisher
    {
        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        void Publish<T>(T eventMessage) where T : IInternalEvent;

        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        Task PublishAsync<T>(T eventMessage) where T : IInternalAsyncEvent;

        /// <summary>
        /// Publish event async and forget it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        void FireAndForget<T>(T eventMessage) where T : IInternalEvent;

        /// <summary>
        /// Publish event async and forget it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        Task FireAndForgetAsync<T>(T eventMessage) where T : IInternalAsyncEvent;
    }
}
