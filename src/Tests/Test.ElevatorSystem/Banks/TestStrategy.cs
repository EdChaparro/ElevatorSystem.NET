#nullable enable
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
{
    public class TestStrategy : AbstractStrategy
    {
        public TestStrategy(Bank bank, IStrategy? nextStrategy = null) : base(bank, nextStrategy)
        { }

        /// <summary>
        /// For testing only -- arbitrarily assigns first floor stop
        /// </summary>
        /// <param name="floorStops"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected override IList<RequestedFloorStop> DoElevatorAssignments
            (IList<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<RequestedFloorStop>();

            if (!floorStops.Any())
            {
                return assignedFloorStops;
            }

            var rfs = new RequestedFloorStop(floorStops.First(), direction);
            assignedFloorStops.Add(rfs);

            return assignedFloorStops;
        }
    }
}