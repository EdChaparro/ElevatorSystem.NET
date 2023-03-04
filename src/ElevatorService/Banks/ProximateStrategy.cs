using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Banks
{
    /// <summary>
    /// Assigns elevators already in route to pending floor stop calls.
    /// </summary>
    public class ProximateStrategy : AbstractStrategy
    {
        public ProximateStrategy(Bank bank, IStrategy? nextStrategy = null) : base(bank, nextStrategy)
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

                var result = proximateElevator.RequestStopAtFloorNumber(floorNbr, direction);

                if (result.requestedFloorStop != null)
                {
                    assignedFloorStops.Add(result.requestedFloorStop);
                }
            }

            return assignedFloorStops;
        }

        protected Elevator? FindProximateElevator(int floorNbr, Direction direction)
        {
            IEnumerable<Elevator> elevators;

            switch (direction)
            {
                case Direction.Down:
                    elevators = Bank.Elevators
                        .Where(x =>
                            x.IsEnabled &&
                            x.Direction == direction &&
                            x.CurrentFloorNumber > floorNbr)
                        .OrderByDescending(x => x.CurrentFloorNumber);
                    break;
                default:
                    elevators = Bank.Elevators
                        .Where(x =>
                            x.IsEnabled &&
                            x.Direction == direction &&
                            x.CurrentFloorNumber < floorNbr)
                        .OrderByDescending(x => x.CurrentFloorNumber);
                    break;
            }

            Elevator? elevator = null;
            var oppositeDirection = direction == Direction.Down ? Direction.Up : Direction.Down;


            foreach (var e in elevators)
            {
                if (e.RequestedFloorStops.Any(x => x.Direction == oppositeDirection))
                {
                    continue;
                }

                elevator = e;
                break;
            }

            return elevator;
        }
    }
}