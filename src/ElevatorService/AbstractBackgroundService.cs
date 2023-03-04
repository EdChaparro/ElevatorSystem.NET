namespace IntrepidProducts.ElevatorService;

public interface IBackgroundService : IHostedService
{
    bool IsRunning { get; }
}

public abstract class AbstractBackgroundService : BackgroundService, IBackgroundService
{
    protected AbstractBackgroundService(int sleepIntervalInMilliseconds)
    {
        SleepIntervalInMilliseconds = sleepIntervalInMilliseconds;
    }

    protected int SleepIntervalInMilliseconds { get; set; }

    public bool IsRunning { get; private set; }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        BeforeServiceLoop();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ServiceLoop();
            await Task.Delay(SleepIntervalInMilliseconds, stoppingToken);
        }
    }

    protected virtual void BeforeServiceLoop()
    { }

    protected abstract void ServiceLoop();

}