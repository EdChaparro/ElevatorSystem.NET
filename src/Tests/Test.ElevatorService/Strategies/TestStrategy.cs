#nullable enable
using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Strategies
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

        public static void WaitForElevatorToReachFloor
            (int floorNbr, Elevator elevator, int timeOutInSeconds = 10)
        {
            if (elevator.CurrentFloorNumber == floorNbr &&
                elevator.DoorStatus == DoorStatus.Open)
            {
                return;     //All done
            }

            var isFloorReached = false;

            elevator.DoorStateChangedEvent += (sender, e)
                =>
            {
                if (elevator.DoorStatus == DoorStatus.Closed)
                {
                    return;
                }

                if (elevator.CurrentFloorNumber == floorNbr)
                {
                    isFloorReached = true;
                }
            };

            var timeOut = DateTime.Now + new TimeSpan(0, 0, timeOutInSeconds);
            var isTimeOut = false;

            while (!isTimeOut)
            {
                if (isFloorReached)
                {
                    break;
                }

                if (DateTime.Now > timeOut)
                {
                    isTimeOut = true;
                }

                Thread.Sleep(500);
            }

            Console.WriteLine($"Elevator CurrentFloorNumber: {elevator.CurrentFloorNumber}");
            Assert.IsFalse(isTimeOut);

            //Added to resolved test flakiness. Some Elevator Events were
            //unpredictably firing after test assertions, leading to random failures.
            Thread.Sleep(100);
        }

    }
}