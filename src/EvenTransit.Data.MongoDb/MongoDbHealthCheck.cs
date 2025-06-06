﻿using EvenTransit.Data.MongoDb.Abstractions;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly MongoDbSettings _settings;
    private readonly IMongoClientProvider _mongoClientProvider;

    public MongoDbHealthCheck(IOptions<MongoDbSettings> settings,
        IMongoClientProvider mongoClientProvider)
    {
        _mongoClientProvider = mongoClientProvider;
        _settings = settings.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var database = _mongoClientProvider.Client.GetDatabase(_settings.Database);
            var isConnected = database
                .RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cancellationToken)
                .Wait(1000, cancellationToken);

            var collection = database.GetCollection<LogStatistic>("LogStatistic");
            var document = collection.Find(_ => true).FirstOrDefault(cancellationToken);
            
            return isConnected
                ? Task.FromResult(HealthCheckResult.Healthy())
                : Task.FromResult(HealthCheckResult.Unhealthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, exception: ex));
        }
    }
}
