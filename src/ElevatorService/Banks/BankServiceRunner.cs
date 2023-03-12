using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Banks
{
    public class BankServiceRunner : IServiceRunner<Bank>
    {
        public BankServiceRunner(IBankServiceRegistry registry, IServiceRunner<Elevator> elevatorRunner)
        {
            _registry = registry;
            _elevatorRunner = elevatorRunner;
        }

        private readonly IBankServiceRegistry _registry;
        private readonly IServiceRunner<Elevator> _elevatorRunner;

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

            foreach (var elevator in bank.EnabledElevators)
            {
                _elevatorRunner.Start(elevator);
            }

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

            var runningElevators = new List<Elevator>();

            foreach (var elevator in bank.Elevators)
            {
                if (_elevatorRunner.IsRunning(elevator))
                {
                    runningElevators.Add(elevator);
                }
            }

            await Task.WhenAll(
                runningElevators.Select(x => _elevatorRunner.StopAsync(x))
            );

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