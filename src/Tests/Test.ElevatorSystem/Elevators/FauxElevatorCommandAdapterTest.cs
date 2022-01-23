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
            var e = new Elevator(1, 2);
            var adapter = new FauxElevatorCommandAdapter(e);

            e.Direction = Direction.Up;
            Assert.AreEqual(e.Direction, adapter.Status.Direction);
            e.Direction = Direction.Down;
            Assert.AreEqual(e.Direction, adapter.Status.Direction);

            e.FloorNumber = 1;
            Assert.AreEqual(e.FloorNumber, adapter.Status.CurrentFloorNumber);
            e.FloorNumber = 2;
            Assert.AreEqual(e.FloorNumber, adapter.Status.CurrentFloorNumber);

            e.DoorStatus = DoorStatus.Open;
            Assert.AreEqual(e.DoorStatus, adapter.Status.DoorStatus);
            e.DoorStatus = DoorStatus.Closed;
            Assert.AreEqual(e.DoorStatus, adapter.Status.DoorStatus);
        }
    }
}