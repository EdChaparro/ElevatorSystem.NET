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
            var requestedFloorStopsFromAssignments =
                DoElevatorAssignments(floorStops, direction);

            if ((NextStrategy == null) || (!requestedFloorStopsFromAssignments.Any()))
            {
                return requestedFloorStopsFromAssignments;
            }

            var remainingUnassignedFloorStops
                = floorStops.Except(requestedFloorStopsFromAssignments
                    .Select(x => x.FloorNbr)).ToList();

            if (!remainingUnassignedFloorStops.Any())
            {
                return requestedFloorStopsFromAssignments;
            }

            return NextStrategy.AssignElevators(remainingUnassignedFloorStops, direction);
        }

        protected abstract IList<RequestedFloorStop> DoElevatorAssignments
            (IList<int> floorStops, Direction direction);

        public IStrategy? NextStrategy { get; }
    }
}