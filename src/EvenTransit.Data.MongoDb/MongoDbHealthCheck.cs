using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly MongoDbSettings _settings;
    private readonly MongoDbConnectionStringBuilder _mongoDbConnectionStringBuilder;

    public MongoDbHealthCheck(IOptions<MongoDbSettings> settings, MongoDbConnectionStringBuilder mongoDbConnectionStringBuilder)
    {
        _mongoDbConnectionStringBuilder = mongoDbConnectionStringBuilder;
        _settings = settings.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var client = new MongoClient(_mongoDbConnectionStringBuilder.ConnectionString);
            var database = client.GetDatabase(_settings.Database);
            var isConnected = database
                .RunCommandAsync((Command<BsonDocument>) "{ping:1}", cancellationToken: cancellationToken)
                .Wait(1000, cancellationToken);

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
