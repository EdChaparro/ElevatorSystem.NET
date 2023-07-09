using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorService.Banks
{
    public interface IBankServiceRegistry
    {
        void Register(params Bank[] banks);
        void UnRegister(params Bank[] banks);

        bool IsRegistered(Bank bank);
        bool IsRegistered(Guid bankId);

        int Count { get; }

        IBackgroundService? Get(Bank bank);

        IBackgroundService? Get(Guid bankId);

        bool IsRunning(Guid bankId);
    }

    public class BankServiceRegistry : IBankServiceRegistry
    {
        public BankServiceRegistry(IElevatorServiceRegistry elevatorServiceRegistry)
        {
            _elevatorServiceRegistry = elevatorServiceRegistry;
        }

        private readonly IElevatorServiceRegistry _elevatorServiceRegistry;

        private readonly Dictionary<Guid, (BankService service, CancellationToken cancellationToken)>
            _serviceDetails = new();

        public int Count => _serviceDetails.Count;

        public void Register(params Bank[] banks)
        {
            foreach (var bank in banks)
            {
                if (_serviceDetails.ContainsKey(bank.Id))
                {
                    continue;
                }

                //TODO: Use IoC
                _serviceDetails.Add(bank.Id,
                    (new BankService(bank, _elevatorServiceRegistry), new CancellationToken()));

                foreach (var elevator in bank.EnabledElevators)
                {
                    _elevatorServiceRegistry.Register(elevator);
                }
            }
        }

        public IBackgroundService? Get(Guid bankId)
        {
            return _serviceDetails.ContainsKey(bankId) ? _serviceDetails[bankId].service : null;
        }

        public IBackgroundService? Get(Bank bank)
        {
            return Get(bank.Id);
        }

        public void UnRegister(params Bank[] banks)
        {
            foreach (var bank in banks)
            {
                if (!_serviceDetails.ContainsKey(bank.Id))
                {
                    continue;
                }

                var (service, cancellationToken) = _serviceDetails[bank.Id];

                if (service.IsRunning)
                {
                    //TODO: Not a safe way to stop since since we're not going through the Runner
                    service.StopAsync(cancellationToken);
                }

                _serviceDetails.Remove(bank.Id);

                foreach (var elevator in bank.Elevators)
                {
                    _elevatorServiceRegistry.UnRegister(elevator);
                }
            }
        }

        public bool IsRegistered(Guid bankId)
        {
            return _serviceDetails.ContainsKey(bankId);
        }

        public bool IsRegistered(Bank bank)
        {
            return IsRegistered(bank.Id);
        }

        public bool IsRunning(Guid bankId)
        {
            var service = Get(bankId);

            return service?.IsRunning ?? false;
        }
    }
}