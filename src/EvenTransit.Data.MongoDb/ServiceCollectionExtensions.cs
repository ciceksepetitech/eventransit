using EvenTransit.Data.MongoDb.Repositories;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvenTransit.Data.MongoDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEventsRepository, EventsMongoRepository>();
            services.AddScoped<ILogsRepository, LogsMongoRepository>();
            services.AddScoped<ILogStatisticsRepository, LogStatisticsMongoRepository>();
            services.AddScoped<IEventLogStatisticRepository, EventLogStatisticMongoRepository>();

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

            return services;
        }
    }
}