using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Banks
{
    //TODO: Consider creating ONE Service to manage multiple Banks
    public class BankService : AbstractBackgroundService, IEntityBackgroundService<Bank>
    {
        public BankService(Bank bank, IElevatorServiceRegistry elevatorRegistry)
            : base(Configuration.EngineSleepIntervalInMilliseconds)
        {
            Bank = bank;
            _elevatorRegistry = elevatorRegistry;

            Strategy = new IdleStrategy(bank, new ProximateStrategy(bank));  //TODO: use IoC
        }

        private Bank Bank { get; }
        public Bank Entity => Bank;


        private readonly IElevatorServiceRegistry _elevatorRegistry;
        private IStrategy Strategy { get; }

        public List<RequestedFloorStop> AssignedFloorStops { get; private set; } = new();

        protected override void BeforeStart()
        {

            foreach (var elevator in Bank.EnabledElevators)
            {
                var elevatorService = _elevatorRegistry.Get(elevator.Id);

                elevatorService?.StartAsync(CancellationToken);
                elevator.RequestStopAtFloorNumber(Bank.LowestFloorNbr);
            }
        }

        protected override void BeforeStop()
        {
            foreach (var elevator in Bank.EnabledElevators)
            {
                var elevatorService = _elevatorRegistry.Get(elevator.Id);

                elevatorService?.StopAsync(CancellationToken);
            }
        }

        protected override void ServiceLoop()
        {
            ClearCompletedFloorStops();

            AssignedFloorStops.AddRange(AssignElevators(Direction.Down));
            AssignedFloorStops.AddRange(AssignElevators(Direction.Up));
        }

        private void ClearCompletedFloorStops()
        {
            var expiredAssignments =
                AssignedFloorStops.Except(PendingElevatorStops);

            AssignedFloorStops = AssignedFloorStops.Except(expiredAssignments).ToList();
        }

        private IEnumerable<RequestedFloorStop> PendingElevatorStops
        {
            get
            {
                var pendingElevatorStops = new List<RequestedFloorStop>();

                foreach (var e in Bank.Elevators)
                {
                    pendingElevatorStops.AddRange(e.RequestedFloorStops.ToList());
                }

                return pendingElevatorStops;
            }
        }

        private IEnumerable<RequestedFloorStop> AssignElevators(Direction direction)
        {
            var requestedFloorStops = Bank.GetRequestedFloorStops(direction)
                .Select(x => x.FloorNbr).ToList();

            return Strategy.AssignElevators(requestedFloorStops, direction);
        }
    }
}