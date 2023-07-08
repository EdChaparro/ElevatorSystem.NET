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

        public IBackgroundService? Get(Bank bank)
        {
            return _serviceDetails.ContainsKey(bank.Id) ? _serviceDetails[bank.Id].service : null;
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
    }
}