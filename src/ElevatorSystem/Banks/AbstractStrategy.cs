using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    //TODO: Add support for multiple strategies using Chain-of-Responsibility pattern
    public interface IStrategy
    {
        void AssignElevators();
    }

    public abstract class AbstractStrategy : IStrategy
    {
        protected AbstractStrategy(Bank bank)
        {
            Bank = bank;
        }

        protected Bank Bank { get; }

        private readonly List<RequestedFloorStop> _assignedDownRequestedFloorStops
            = new List<RequestedFloorStop>();
        public List<RequestedFloorStop> AssignedDownRequestedFloorStops
            => _assignedDownRequestedFloorStops.ToList();

        private readonly List<RequestedFloorStop> _assignedUpRequestedFloorStops
            = new List<RequestedFloorStop>();
        public List<RequestedFloorStop> AssignedUpRequestedFloorStops
            => _assignedUpRequestedFloorStops.ToList();

        public void AssignElevators()
        {
            var assignedDown = Assign(Direction.Down);
            var assignedUp = Assign(Direction.Up);

            _assignedDownRequestedFloorStops.AddRange(assignedDown);
            _assignedUpRequestedFloorStops.AddRange(assignedUp);
        }

        private IEnumerable<RequestedFloorStop> Assign(Direction direction)
        {
            var floorStopRequests
                = FindFloorStopsRequiringService
                        (Bank.GetRequestedFloorStops(direction), direction).ToList();

            var floorNumbersToPassAlong = new List<int>();
            foreach (var floorNbr in floorStopRequests)
            {
                switch (direction)
                {
                    case Direction.Down:
                        if (AssignedDownRequestedFloorStops.Any(x => x.FloorNbr == floorNbr))
                        {
                            continue;
                        }

                        floorNumbersToPassAlong.Add(floorNbr);
                        break;

                    case Direction.Up:
                        if (AssignedUpRequestedFloorStops.Any(x => x.FloorNbr == floorNbr))
                        {
                            continue;
                        }

                        floorNumbersToPassAlong.Add(floorNbr);
                        break;
                }
            }

            return Assign(floorNumbersToPassAlong, direction);
        }

        protected abstract IEnumerable<RequestedFloorStop> Assign
            (IEnumerable<int> floorStops, Direction direction);

        protected IEnumerable<int> FindFloorStopsRequiringService
            (IEnumerable<RequestedFloorStop> requestedFloorStops, Direction direction)
        {
            var floorStopsRequiringService = new HashSet<int>();

            foreach (var requestedFloorStop in requestedFloorStops)
            {
                if (Bank.IsElevatorStoppingAtFloorFromDirection(requestedFloorStop.FloorNbr, direction))
                {
                    continue;   //Skip stops already scheduled to be visited by an elevator
                }

                floorStopsRequiringService.Add(requestedFloorStop.FloorNbr);
            }

            switch (direction)
            {
                case Direction.Down:
                    return floorStopsRequiringService.OrderByDescending(x => x);
                default:
                    return floorStopsRequiringService.OrderBy(x => x);
            }
        }
    }
}