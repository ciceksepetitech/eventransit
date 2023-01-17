using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Service.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace EvenTransit.Service.BackgroundServices;

/// <summary>
/// Event publisher
/// </summary>
public class InternalEventPublisher : IInternalEventPublisher
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IAsyncRunner _asyncRunner;
    private readonly ILogger<InternalEventPublisher> _logger;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="subscriptionService"></param>
    /// <param name="asyncRunner"></param>
    /// <param name="logger">ILogger</param>
    public InternalEventPublisher
    (
        ISubscriptionService subscriptionService,
        IAsyncRunner asyncRunner,
        ILogger<InternalEventPublisher> logger
    )
    {
        _subscriptionService = subscriptionService;
        _asyncRunner = asyncRunner;
        _logger = logger;
    }

    /// <summary>
    /// Publish to consumer
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="consumer">Event consumer</param>
    /// <param name="eventMessage">Event message</param>
    protected virtual void PublishToConsumer<T>(IConsumer<T> consumer, T eventMessage) where T : IInternalEvent
    {
        try
        {
            consumer.HandleEvent(eventMessage);
        }
        catch (Exception exc)
        {
            //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
                _logger.LogError(exc, exc.Message);
            }
            catch (Exception)
            {
                //do nothing
            }
        }
    }

    /// <summary>
    /// Publish event
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventMessage">Event message</param>
    public virtual void Publish<T>(T eventMessage) where T : IInternalEvent
    {
        var subscriptions = _subscriptionService.GetSubscriptions<T>();
        subscriptions.ToList().ForEach(x => PublishToConsumer(x, eventMessage));
    }


    /// <summary>
    /// Publish event
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventMessage">Event message</param>
    public virtual async Task PublishAsync<T>(T eventMessage) where T : IInternalAsyncEvent
    {
        var subscriptions = _subscriptionService.GetAsyncSubscriptions<T>();
        foreach (var subscription in subscriptions)
            await PublishToConsumerAsync(subscription, eventMessage);
    }

    /// <summary>
    /// Publish to consumer
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="x">Event consumer</param>
    /// <param name="eventMessage">Event message</param>
    protected virtual async Task PublishToConsumerAsync<T>(IAsyncConsumer<T> x, T eventMessage) where T : IInternalAsyncEvent
    {
        try
        {
            await x.HandleEventAsync(eventMessage);
        }
        catch (Exception exc)
        {
            //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
                _logger.LogError(exc, exc.Message);
            }
            catch (Exception)
            {
                //do nothing
            }
        }
    }


    /// <summary>
    /// Publish event async and forget it
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventMessage">Event message</param>
    public virtual void FireAndForget<T>(T eventMessage) where T : IInternalEvent
    {
        _asyncRunner.Run<IInternalEventPublisher>(p => p.Publish(eventMessage));
    }

    /// <summary>
    /// Publish event async and forget it along with async await support.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventMessage">Event message</param>
    public virtual Task FireAndForgetAsync<T>(T eventMessage) where T : IInternalAsyncEvent
    {
        return _asyncRunner.RunAsync<IInternalEventPublisher>(p => p.PublishAsync(eventMessage));
    }
}
