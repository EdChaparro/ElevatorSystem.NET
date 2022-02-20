using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class FauxElevatorCommandAdapterTest
    {
        [TestMethod]
        public void ShouldTrackStatusOfElevator()
        {
            var bank = new Bank(2, 1..4);

            var elevator = bank.Elevators.First();

            Assert.AreEqual(DoorStatus.Closed, elevator.DoorStatus);

            elevator.RequestStopAtFloorNumber(3);
            Assert.AreEqual(Direction.Up, elevator.Direction);

            elevator.RequestStopAtFloorNumber(2);
            Assert.AreEqual(Direction.Down, elevator.Direction);

            Assert.AreEqual(2, elevator.CurrentFloorNumber);

            Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
        }
    }
}