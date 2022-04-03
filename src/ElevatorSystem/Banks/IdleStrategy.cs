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
        public override void AssignElevators(Bank bank)
        {
            AssignFloorStopsRequiringService(bank, Direction.Down);
            AssignFloorStopsRequiringService(bank, Direction.Up);
        }

        private void AssignFloorStopsRequiringService(Bank bank, Direction direction)
        {
            var floorStopRequests
                = FindFloorStopsRequiringService(bank, bank.RequestedFloorStops(direction), direction)
                    .ToList();

            //First, try to assign floor stop requests to idle elevators
            var assignedFloorNbrs = AssignIdleElevators(bank, floorStopRequests, direction);
            foreach (var assignedFloorNbr in assignedFloorNbrs)
            {
                floorStopRequests.Remove(assignedFloorNbr);
            }
        }

        private List<int> AssignIdleElevators(Bank bank, IList<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<int>();

            foreach (var floorNbr in floorStops)
            {
                var idleElevator = FindIdleElevator(bank, direction);

                if (idleElevator == null)
                {
                    break;
                }

                assignedFloorStops.Add(floorNbr);
                idleElevator.RequestStopAtFloorNumber(floorNbr, true);
            }

            return assignedFloorStops;
        }
    }
}