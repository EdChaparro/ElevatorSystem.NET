using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Strategies
{
    [TestClass]
    public class IdleStrategyTest
    {
        [TestMethod]
        public void ShouldAssignFirstAvailableElevator()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();

            Assert.IsTrue(bank.PressButtonForFloorNumber(8, Direction.Down));

            var strategy = new IdleStrategy(bank);

            var assignments = strategy.AssignElevators
                (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                    Direction.Down).ToList();

            Assert.AreEqual(1, assignments.Count());
            var rfs = assignments.First();

            Assert.AreEqual(1, elevator1.RequestedFloorStops.Count());
            Assert.AreEqual(rfs, elevator1.RequestedFloorStops.First());
        }

        [TestMethod]
        public void ShouldNotAssignWhenElevatorsAreBusy()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            var strategy = new ProximateStrategy(bank);

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(8).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(bank.PressButtonForFloorNumber(7, Direction.Down));

            var assignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();

            Assert.AreEqual(0, assignments.Count());
        }
    }
}