using System.Collections.Generic;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    //TODO: Add support for multiple strategies using Chain-of-Responsibility pattern
    public interface IStrategy
    {
        IEnumerable<RequestedFloorStop> AssignElevators
            (IEnumerable<int> floorStopRequests, Direction direction);
    }

    public abstract class AbstractStrategy : IStrategy
    {
        protected AbstractStrategy(Bank bank)
        {
            Bank = bank;
        }

        protected Bank Bank { get; }

        public abstract IEnumerable<RequestedFloorStop> AssignElevators
            (IEnumerable<int> floorStops, Direction direction);
    }
}