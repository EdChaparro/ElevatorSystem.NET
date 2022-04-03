using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    /// <summary>
    /// Assigns idle elevators to pending floor stop calls.
    /// </summary>
    public class IdleStrategy : AbstractStrategy
    {
        protected override IEnumerable<RequestedFloorStop> Assign(Bank bank, IEnumerable<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<RequestedFloorStop>();

            var assignedFloorNbrs = AssignIdleElevators(bank, floorStops, direction);
            foreach (var assignedFloorNbr in assignedFloorNbrs)
            {
                assignedFloorStops.Add(assignedFloorNbr);
            }

            return assignedFloorStops;
        }

        private List<RequestedFloorStop> AssignIdleElevators(Bank bank, IEnumerable<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<RequestedFloorStop>();

            foreach (var floorNbr in floorStops)
            {
                var idleElevator = FindIdleElevator(bank, direction);

                if (idleElevator == null)
                {
                    break;
                }

                var result = idleElevator.RequestStopAtFloorNumber(floorNbr);

                if (result.requestedFloorStop != null)
                {
                    assignedFloorStops.Add(result.requestedFloorStop);
                }
            }

            return assignedFloorStops;
        }

        protected Elevator? FindIdleElevator(Bank bank, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return bank.IdleElevators
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return bank.IdleElevators
                        .OrderBy(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }
    }
}