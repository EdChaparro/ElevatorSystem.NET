using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    /// <summary>
    /// Assigns elevators already in route to pending floor stop calls.
    /// </summary>
    public class ProximateStrategy : AbstractStrategy
    {
        public ProximateStrategy(Bank bank) : base(bank)
        { }

        protected override IList<RequestedFloorStop> DoElevatorAssignments(IList<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<RequestedFloorStop>();

            foreach (var floorNbr in floorStops)
            {
                var proximateElevator = FindProximateElevator(floorNbr, direction);

                if (proximateElevator == null)
                {
                    break;
                }

                var result = proximateElevator.RequestStopAtFloorNumber(floorNbr);

                if (result.requestedFloorStop != null)
                {
                    assignedFloorStops.Add(result.requestedFloorStop);
                }
            }

            return assignedFloorStops;
        }

        protected Elevator? FindProximateElevator(int floorNbr, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Bank.Elevators
                        .Where(x =>
                            x.IsEnabled &&
                            x.Direction == direction &&
                            x.CurrentFloorNumber > floorNbr)
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return Bank.Elevators
                        .Where(x =>
                            x.IsEnabled &&
                            x.Direction == direction &&
                            x.CurrentFloorNumber < floorNbr)
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }
    }
}