using System.Linq;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Tests.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
{
    [TestClass]
    public class ProximateStrategyTest
    {
        [TestMethod]
        public void ShouldAssignProximateElevatorToServiceStopRequests()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            elevator1.Start();
            elevator2.Start();

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(8).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(bank.PressButtonForFloorNumber(7, Direction.Up));

            var strategy = new ProximateStrategy(bank);

            var downAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();
            Assert.AreEqual(0, downAssignments.Count);

            var upAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Up).ToList();
            Assert.AreEqual(1, upAssignments.Count());
            var sRFS = upAssignments.First();
            var eRFS = elevator1.RequestedFloorStops
                .Select(x => x.Id == sRFS.Id).FirstOrDefault();
            Assert.IsNotNull(eRFS);

            ElevatorTest.WaitForElevatorToReachFloor(5, elevator2);
            Assert.IsTrue(elevator2.PressButtonForFloorNumber(10));
            ElevatorTest.WaitForElevatorToReachFloor(10, elevator2);

            Assert.IsTrue(elevator2.PressButtonForFloorNumber(1));
            bank.PressButtonForFloorNumber(4, Direction.Down);
            downAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();
            Assert.AreEqual(1, downAssignments.Count);
            sRFS = downAssignments.First();
            eRFS = elevator2.RequestedFloorStops
                .Select(x => x.Id == sRFS.Id).FirstOrDefault();
            Assert.IsNotNull(eRFS);

            elevator1.Stop();
            elevator2.Stop();
        }

        [TestMethod]
        public void ShouldNotAssignWhenNoProximateElevatorsAreAvailable()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            var strategy = new ProximateStrategy(bank);

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(6).isOk);
            Assert.IsTrue(bank.PressButtonForFloorNumber(2, Direction.Down));

            var assignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();

            Assert.AreEqual(0, assignments.Count);
        }
    }
}