namespace EvenTransit.Logging.Serilog;

public class Bootstrapper
{
    public static void Run<TLogger>(Action action) where TLogger : IBootstrapLogger, new()
    {
        Run<TLogger, object>(action, null);
    }

    public static void Run<TLogger, TConfiguration>(Action action, Action<TConfiguration> config)
        where TLogger : IBootstrapLogger, new()
    {
        var logger = new TLogger();

        logger.Configure(config);

        try
        {
            logger.LogInformation("Starting host.");
            action();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Host terminated unexpectedly.");
            throw;
        }
        finally
        {
            logger.Dispose();
        }
    }
}
