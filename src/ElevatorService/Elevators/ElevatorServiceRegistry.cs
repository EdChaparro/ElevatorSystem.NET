using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Elevators
{
    public interface IElevatorServiceRegistry
    {
        void Register(params Elevator[] elevators);
        void UnRegister(params Elevator[] elevators);

        bool IsRegistered(Elevator elevator);
        bool IsRegistered(Guid elevatorId);

        int Count { get; }

        IEntityBackgroundService<Elevator>? Get(Elevator elevator);
        IEntityBackgroundService<Elevator>? Get(Guid elevatorId);

        bool IsRunning(Guid bankId);

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

        public IEntityBackgroundService<Elevator>? Get(Guid elevatorId)
        {
            return _serviceDetails.ContainsKey(elevatorId) ? _serviceDetails[elevatorId].service : null;
        }

        public IEntityBackgroundService<Elevator>? Get(Elevator elevator)
        {
            return Get(elevator.Id);
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

        public bool IsRegistered(Guid elevatorId)
        {
            return _serviceDetails.ContainsKey(elevatorId);
        }

        public bool IsRegistered(Elevator elevator)
        {
            return IsRegistered(elevator.Id);
        }

        public bool IsRunning(Guid elevatorId)
        {
            var service = Get(elevatorId);

            return service?.IsRunning ?? false;
        }
    }
}