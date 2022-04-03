using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    //TODO: Add support for multiple strategies using Chain-of-Responsibility pattern
    public interface IStrategy
    {
        void AssignElevators(Bank bank);
    }

    public abstract class AbstractStrategy : IStrategy
    {
        public abstract void AssignElevators(Bank bank);

        protected IEnumerable<int> FindFloorStopsRequiringService
            (Bank bank, IEnumerable<RequestedFloorStop> requestedFloorStops, Direction direction)
        {
            var floorStopsRequiringService = new HashSet<int>();

            foreach (var requestedFloorStop in requestedFloorStops)
            {
                if (bank.IsElevatorStoppingAtFloorFromDirection(requestedFloorStop.FloorNbr, direction))
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

        protected Elevator? FindIdleElevator(Bank bank, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return bank.IdleElevators
                        .Where(x => !x.IsOnAdministrativeLock)
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return bank.IdleElevators
                        .Where(x => !x.IsOnAdministrativeLock)
                        .OrderBy(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }
    }
}