using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Elevators
{
    public class ElevatorServiceRunner
    {
        public ElevatorServiceRunner(IElevatorServiceRegistry registry)
        {
            _registry = registry;
        }

        private readonly IElevatorServiceRegistry _registry;

        private readonly Dictionary<Guid, (IBackgroundService service, CancellationToken cancellationToken)>
            _runningServices = new();


        public bool Start(Elevator elevator)
        {
            if (!elevator.IsEnabled)
            {
                return false;
            }

            var service = _registry.Get(elevator);

            if (service == null)
            {
                return false;
            }

            var cancellationToken = new CancellationToken();
            _runningServices.Add(elevator.Id, (service, cancellationToken));

            service.StartAsync(cancellationToken);
            return true;
        }

        public async Task<bool> StopAsync(Elevator elevator)
        {
            if (!_runningServices.ContainsKey(elevator.Id))
            {
                return false;
            }

            var (service, cancellationToken) = _runningServices[elevator.Id];

            if (!service.IsRunning)
            {
                return true;
            }

            await service.StopAsync(cancellationToken);

            _runningServices.Remove(elevator.Id);
            return true;
        }

        public bool IsRunning(Elevator elevator)
        {
            return _runningServices.ContainsKey(elevator.Id) &&
                   _runningServices[elevator.Id].service.IsRunning;
        }
    }
}