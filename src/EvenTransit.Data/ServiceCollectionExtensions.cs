using EvenTransit.Data.Abstractions;
using EvenTransit.Data.Repositories;
using EvenTransit.Data.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEventsRepository, EventsMongoRepository>();
            services.AddScoped<ILogsRepository, LogsMongoRepository>();

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

            return services;
        }
    }
}