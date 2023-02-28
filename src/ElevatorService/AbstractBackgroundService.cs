namespace IntrepidProducts.ElevatorService;

public interface IBackgroundService : IHostedService
{
    bool IsRunning { get; }
}

public abstract class AbstractBackgroundService : BackgroundService, IBackgroundService
{
    public bool IsRunning { get; private set; }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return base.StopAsync(cancellationToken);
    }
}