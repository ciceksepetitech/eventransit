namespace EvenTransit.Messaging.Core.Abstractions;

public interface IAsyncConsumer<T> where T : IInternalAsyncEvent
{
    Task HandleEventAsync(T eventMessage);
}

public interface IInternalAsyncEvent
{
}
