namespace DETechOne.SmartWMS.Tasks;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SmartWMS Task Engine started at {UtcTime}.", DateTime.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("SmartWMS Task Engine heartbeat at {UtcTime}.", DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
