using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    public interface IStrategy
    {
        IEnumerable<RequestedFloorStop> AssignElevators
            (IList<int> floorStopRequests, Direction direction);

        IStrategy? NextStrategy { get; }
    }

    public abstract class AbstractStrategy : IStrategy
    {
        protected AbstractStrategy(Bank bank, IStrategy? nextStrategy = null)
        {
            Bank = bank;
            NextStrategy = nextStrategy;
        }

        protected Bank Bank { get; }

        public IEnumerable<RequestedFloorStop> AssignElevators
            (IList<int> floorStops, Direction direction)
        {
            var floorStopsAssignedByThisStrategy =
                DoElevatorAssignments(floorStops, direction);

            if (NextStrategy == null)
            {
                return floorStopsAssignedByThisStrategy;
            }

            var remainingUnassignedFloorStops
                = floorStops.Except(floorStopsAssignedByThisStrategy
                    .Select(x => x.FloorNbr)).ToList();

            if (!remainingUnassignedFloorStops.Any())
            {
                return floorStopsAssignedByThisStrategy;
            }

            var floorStopsAssignedByOtherStrategies = NextStrategy.AssignElevators(remainingUnassignedFloorStops, direction);

            return floorStopsAssignedByThisStrategy.Concat(floorStopsAssignedByOtherStrategies);
        }

        protected abstract IList<RequestedFloorStop> DoElevatorAssignments
            (IList<int> floorStops, Direction direction);

        public IStrategy? NextStrategy { get; }
    }
}