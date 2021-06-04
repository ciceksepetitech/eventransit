using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Core.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}