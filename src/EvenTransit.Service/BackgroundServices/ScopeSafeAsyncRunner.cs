using EvenTransit.Service.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Service.BackgroundServices;

/// <summary>
/// ScopeSafeAsyncRunner
/// </summary>
public class ScopeSafeAsyncRunner : IAsyncRunner
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider</param>
    public ScopeSafeAsyncRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Run the action as async.
    /// </summary>
    /// <param name="action">Action</param>
    /// <typeparam name="T">Type of service to passing inside the action.</typeparam>
    public virtual void Run<T>(Action<T> action) where T : class
    {
        Task.Run(() =>
        {
            // Create a nested container which will use the same dependency
            // registrations as set for HTTP request scopes.
            using var scope = _serviceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<T>();
            action(service);
        });
    }


    /// <summary>
    /// Run the action as async along with async await support.
    /// </summary>
    /// <param name="action">Action Task</param>
    /// <typeparam name="T">Type of service to passing inside the action.</typeparam>
    public virtual Task RunAsync<T>(Func<T, Task> action) where T : class
    {
        Task.Run(async () =>
        {
            // Create a nested container which will use the same dependency
            // registrations as set for HTTP request scopes.
            using var scope = _serviceProvider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<T>();
            await action(service);
        });
        return Task.CompletedTask;
    }
}
