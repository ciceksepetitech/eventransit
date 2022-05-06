using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace EvenTransit.Messaging.RabbitMq;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMqHealthCheck(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var connection = _connectionFactory.CreateConnection();
            var isConnected = connection.IsOpen;
            
            if(connection.IsOpen)
                connection.Close();

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
