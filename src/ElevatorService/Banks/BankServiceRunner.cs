using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorService.Banks
{
    public class BankServiceRunner : IServiceRunner<Bank>
    {
        public BankServiceRunner(IBankServiceRegistry registry)
        {
            _registry = registry;
        }

        private readonly IBankServiceRegistry _registry;

        private readonly Dictionary<Guid, (IBackgroundService service, CancellationToken cancellationToken)>
            _runningServices = new();

        public int Count => _runningServices.Count;


        public bool Start(Bank bank)
        {
            var service = _registry.Get(bank);

            if (service == null)
            {
                return false;
            }

            var cancellationToken = new CancellationToken();
            _runningServices.Add(bank.Id, (service, cancellationToken));

            service.StartAsync(cancellationToken);
            return true;
        }

        public async Task<bool> StopAsync(Bank bank)
        {
            if (!_runningServices.ContainsKey(bank.Id))
            {
                return false;
            }

            var (service, cancellationToken) = _runningServices[bank.Id];

            if (!service.IsRunning)
            {
                return true;
            }

            await service.StopAsync(cancellationToken);
            _runningServices.Remove(bank.Id);
            return true;
        }

        public bool IsRunning(Bank bank)
        {
            return _runningServices.ContainsKey(bank.Id) &&
                   _runningServices[bank.Id].service.IsRunning;
        }
    }
}