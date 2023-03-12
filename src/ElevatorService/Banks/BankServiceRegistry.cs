using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorService.Banks
{
    public interface IBankServiceRegistry
    {
        void Register(params Bank[] banks);
        void UnRegister(params Bank[] banks);

        bool IsRegistered(Bank bank);
        int Count { get; }

        IBackgroundService? Get(Bank bank);
    }

    public class BankServiceRegistry : IBankServiceRegistry
    {
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
                _serviceDetails.Add(bank.Id, (new BankService(bank), new CancellationToken()));
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
                    service.StopAsync(cancellationToken);
                }

                _serviceDetails.Remove(bank.Id);
            }
        }

        public bool IsRegistered(Bank bank)
        {
            return _serviceDetails.ContainsKey(bank.Id);
        }
    }
}