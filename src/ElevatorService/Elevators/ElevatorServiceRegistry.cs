using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Elevators
{
    public interface IElevatorServiceRegistry
    {
        void Register(params Elevator[] elevators);
        void UnRegister(params Elevator[] elevators);

        bool IsRegistered(Elevator elevator);
        int Count { get; }

        IBackgroundService? Get(Elevator elevator);
    }

    public class ElevatorServiceRegistry : IElevatorServiceRegistry
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
    }
}