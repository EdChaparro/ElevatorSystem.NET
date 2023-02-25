namespace IntrepidProducts.ElevatorService
{
    public class BankService : BackgroundService
    {
        private readonly ILogger<BankService> _logger;

        public BankService(ILogger<BankService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("BankService running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}