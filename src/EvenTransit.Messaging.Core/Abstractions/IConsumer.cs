namespace EvenTransit.Messaging.Core.Abstractions;

public interface IConsumer<T> where T : IInternalEvent
{
    void HandleEvent(T eventMessage);
}

public interface IInternalEvent
{
}
