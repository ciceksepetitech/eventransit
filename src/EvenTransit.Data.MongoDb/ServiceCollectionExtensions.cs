using EvenTransit.Data.MongoDb.Abstractions;
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
        services.AddSingleton<IMongoClientProvider, MongoClientProvider>();

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(typeof(DateTime), new ApplicationDateTimeSerializer());

        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

        services.AddSingleton<MongoDbConnectionStringBuilder>();

        services.AddHealthChecks()
            .AddCheck<MongoDbHealthCheck>("mongodb");

        return services;
    }
}

public class ApplicationDateTimeSerializer : DateTimeSerializer
{
    //  MongoDB returns datetime as DateTimeKind.Utc, which can't be used in our timezone conversion logic
    //  We overwrite it to be DateTimeKind.Unspecified
    public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var obj = base.Deserialize(context, args);
        return new DateTime(obj.Ticks, DateTimeKind.Unspecified);
    }

    //  MongoDB stores all datetime as Utc, any datetime value DateTimeKind is not DateTimeKind.Utc,
    //  will be converted to Utc first
    //  We overwrite it to be DateTimeKind.Utc, because we want to preserve the raw value
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
    {
        var utcValue = new DateTime(value.Ticks, DateTimeKind.Utc);
        base.Serialize(context, args, utcValue);
    }
}
