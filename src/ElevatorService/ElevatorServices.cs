using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService
{
    public interface IElevatorServices
    {
        int Count { get; }

        void Register(params Elevator[] elevators);
        void UnRegister(params Elevator[] elevators);
        bool IsRegistered(Elevator elevator);

        bool Start(Elevator elevator);
        Task<bool> StopAsync(Elevator elevator);

        bool IsRunning(Elevator elevator);
    }

    public class ElevatorServices : IElevatorServices
    {
        private readonly Dictionary<Guid, (ElevatorService service, CancellationToken cancellationToken)>
            _serviceDetails = new();

        public int Count => _serviceDetails.Count;

        public void Register(params Elevator[] elevators)
        {
            foreach (var elevator in elevators)
            {
                if (_serviceDetails.ContainsKey(elevator.Id))
                {
                    continue;
                }

                //TODO: Use IoC
                _serviceDetails.Add(elevator.Id, (new ElevatorService(elevator), new CancellationToken()));
            }
        }

        public IBackgroundService? Get(Elevator elevator)
        {
            return _serviceDetails.ContainsKey(elevator.Id) ? _serviceDetails[elevator.Id].service : null;
        }

        public void UnRegister(params Elevator[] elevators)
        {
            foreach (var elevator in elevators)
            {
                if (!_serviceDetails.ContainsKey(elevator.Id))
                {
                    continue;
                }

                var (service, cancellationToken) = _serviceDetails[elevator.Id];

                if (service.IsRunning)
                {
                    service.StopAsync(cancellationToken);
                }

                _serviceDetails.Remove(elevator.Id);
            }
        }

        public bool IsRegistered(Elevator elevator)
        {
            return _serviceDetails.ContainsKey(elevator.Id);
        }

        public bool Start(Elevator elevator)
        {
            if (!_serviceDetails.ContainsKey(elevator.Id))
            {
                return false;
            }

            var (service, cancellationToken) = _serviceDetails[elevator.Id];
            service.StartAsync(cancellationToken);
            return true;
        }

        public async Task<bool> StopAsync(Elevator elevator)
        {
            if (!_serviceDetails.ContainsKey(elevator.Id))
            {
                return false;
            }

            var (service, cancellationToken) = _serviceDetails[elevator.Id];

            if (!service.IsRunning)
            {
                return true;
            }

            await service.StopAsync(cancellationToken);
            return true;
        }

        public bool IsRunning(Elevator elevator)
        {
            return _serviceDetails.ContainsKey(elevator.Id) &&
                   _serviceDetails[elevator.Id].service.IsRunning;
        }
    }
}