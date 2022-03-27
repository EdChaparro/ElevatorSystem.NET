using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
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
        }

        private Bank Bank { get; }

        protected override void DoEngineLoop()
        {
            AssignFloorStopsRequiringService(Direction.Down);
            AssignFloorStopsRequiringService(Direction.Up);
        }

        private void AssignFloorStopsRequiringService(Direction direction)
        {
            var floorStopRequests
                = FindFloorStopsRequiringService(Bank.RequestedFloorStopsDown, direction)
                    .ToList();

            var assignedFloorNbrs = AssignIdleElevators(floorStopRequests, direction);

            foreach (var assignedFloorNbr in assignedFloorNbrs)
            {
                floorStopRequests.Remove(assignedFloorNbr);
            }

            //TODO: Implement second algorithm to assign elevators
        }

        private List<int> AssignIdleElevators(IEnumerable<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<int>();

            foreach (var floorNbr in floorStops)
            {
                var idleElevator = FindIdleElevator(direction);

                if (idleElevator == null)
                {
                    break;
                }

                assignedFloorStops.Add(floorNbr);
                idleElevator.RequestStopAtFloorNumber(floorNbr);
            }

            return assignedFloorStops;
        }

        private Elevator? FindIdleElevator(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Bank.IdleElevators
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return Bank.IdleElevators
                        .OrderBy(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }

        private IEnumerable<int> FindFloorStopsRequiringService(IEnumerable<int> requestedFloorStops, Direction direction)
        {
            var floorStopsRequiringService = new HashSet<int>();

            foreach (var requestedFloorStop in requestedFloorStops)
            {
                if (Bank.IsElevatorStoppingAtFloorFromDirection(requestedFloorStop, direction))
                {
                    continue;   //Skip stops already scheduled to be visited by an elevator
                }

                floorStopsRequiringService.Add(requestedFloorStop);
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