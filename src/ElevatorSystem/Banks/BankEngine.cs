using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    /// <summary>
    /// Service loop intended to run in an independent thread.
    /// </summary>
    public class BankEngine : AbstractEngine
    {
        public BankEngine(Bank bank)
        {
            SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;

            Bank = bank;
            Strategy = new IdleStrategy(bank);  //TODO: use IoC
        }

        private Bank Bank { get; }
        private IStrategy Strategy { get; }

        public List<RequestedFloorStop> AssignedFloorStops { get; private set; }
            = new List<RequestedFloorStop>();

        protected override void DoEngineLoop()
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

        private List<RequestedFloorStop> AssignElevators(Direction direction)
        {
            var requestedFloorStops = Bank.GetRequestedFloorStops(direction)
                .Select(x => x.FloorNbr);

            return Strategy.AssignElevators(requestedFloorStops, direction).ToList();
        }
    }
}