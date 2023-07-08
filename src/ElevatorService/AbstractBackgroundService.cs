namespace IntrepidProducts.ElevatorService;

public interface IBackgroundService : IHostedService
{
    bool IsRunning { get; }
    Task StartAsync();
    Task StopAsync();
}

public abstract class AbstractBackgroundService : BackgroundService, IBackgroundService
{
    protected AbstractBackgroundService(int sleepIntervalInMilliseconds)
    {
        SleepIntervalInMilliseconds = sleepIntervalInMilliseconds;
    }

    protected int SleepIntervalInMilliseconds { get; set; }

    public bool IsRunning { get; private set; }
    protected CancellationToken CancellationToken { get; set; }

    public Task StartAsync()
    {
        return StartAsync(new CancellationToken());
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;

        IsRunning = true;
        BeforeStart();
        return base.StartAsync(cancellationToken);
    }

    public Task StopAsync()
    {
        BeforeStop();
        return StopAsync(CancellationToken);
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

    protected virtual void BeforeStart()
    { }

    protected virtual void BeforeStop()
    { }

    protected abstract void ServiceLoop();

}