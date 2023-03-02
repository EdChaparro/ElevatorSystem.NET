using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService;

public class BankService : AbstractBackgroundService
{
    public BankService(Bank bank)
    {
        SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;
        Bank = bank;
        Strategy = new IdleStrategy(bank, new ProximateStrategy(bank));  //TODO: use IoC
    }

    private Bank Bank { get; }
    private IStrategy Strategy { get; }

    protected int SleepIntervalInMilliseconds { get; set; }
    public List<RequestedFloorStop> AssignedFloorStops { get; private set; } = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ClearCompletedFloorStops();

            AssignedFloorStops.AddRange(AssignElevators(Direction.Down));
            AssignedFloorStops.AddRange(AssignElevators(Direction.Up));

            await Task.Delay(SleepIntervalInMilliseconds, stoppingToken);
        }
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

    private List<RequestedFloorStop> AssignElevators(Direction direction)
    {
        var requestedFloorStops = Bank.GetRequestedFloorStops(direction)
            .Select(x => x.FloorNbr).ToList();

        return Strategy.AssignElevators(requestedFloorStops, direction).ToList();
    }
}