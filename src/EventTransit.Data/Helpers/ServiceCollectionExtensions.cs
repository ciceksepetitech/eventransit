using EventTransit.Core.Abstractions.Data;
using EventTransit.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventTransit.Data.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            return services;
        }
    }
}