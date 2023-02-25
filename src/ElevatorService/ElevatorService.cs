namespace IntrepidProducts.ElevatorService;

public class ElevatorService : BackgroundService
{
    private readonly ILogger<ElevatorService> _logger;

    public ElevatorService(ILogger<ElevatorService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("ElevatorService running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}