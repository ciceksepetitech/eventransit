namespace EvenTransit.Service.Abstractions.Events
{
    public interface IAsyncRunner
    {
        void Run<T>(Action<T> action) where T : class;
        Task RunAsync<T>(Func<T, Task> action) where T : class;
    }
}
