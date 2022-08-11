using EvenTransit.Data.MongoDb.Repositories;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace EvenTransit.Data.MongoDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventsRepository, EventsMongoRepository>();
        services.AddScoped<ILogsRepository, LogsMongoRepository>();
        services.AddScoped<ILogStatisticsRepository, LogStatisticsMongoRepository>();
        services.AddScoped<IEventLogStatisticRepository, EventLogStatisticMongoRepository>();
        services.AddScoped<IServiceLockRepository, ServiceLockMongoRepository>();

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

        services.AddSingleton<MongoDbConnectionStringBuilder>();

        services.AddHealthChecks()
            .AddCheck<MongoDbHealthCheck>("mongodb");

        return services;
    }
}
