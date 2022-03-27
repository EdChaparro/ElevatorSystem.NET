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
            //TODO: Consider refactoring by putting strategies in a Chain-of-Responsibility dependency

            AssignFloorStopsRequiringService(Direction.Down);
            AssignFloorStopsRequiringService(Direction.Up);
        }

        private void AssignFloorStopsRequiringService(Direction direction)
        {
            var floorStopRequests
                = FindFloorStopsRequiringService(Bank.RequestedFloorStops(direction), direction)
                    .ToList();

            //First, try to assign floor stop requests to idle elevators
            var assignedFloorNbrs = AssignIdleElevators(floorStopRequests, direction);
            foreach (var assignedFloorNbr in assignedFloorNbrs)
            {
                floorStopRequests.Remove(assignedFloorNbr);
            }

            //TODO: Insert additional strategies
        }

        #region Idle Elevators

        private List<int> AssignIdleElevators(IList<int> floorStops, Direction direction)
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
                idleElevator.RequestStopAtFloorNumber(floorNbr, true);
            }

            return assignedFloorStops;
        }

        private Elevator? FindIdleElevator(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Bank.IdleElevators   //TODO: Modify definition of Idle?
                        .Where(x => !x.IsOnAdministrativeLock)
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return Bank.IdleElevators
                        .Where(x => !x.IsOnAdministrativeLock)
                        .OrderBy(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }
        #endregion

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