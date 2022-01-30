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
            var bank = new Bank(2,
                new Floor(1), new Floor(2),
                new Floor(3), new Floor(4));

            var adapter = bank.ElevatorCommandAdapters.First();

            Assert.AreEqual(DoorStatus.Closed, adapter.Status.DoorStatus);

            adapter.StopAt(3);
            Assert.AreEqual(Direction.Up, adapter.Status.Direction);

            adapter.StopAt(2);
            Assert.AreEqual(Direction.Down, adapter.Status.Direction);

            Assert.AreEqual(2, adapter.Status.CurrentFloorNumber);

            Assert.AreEqual(DoorStatus.Open, adapter.Status.DoorStatus);
        }
    }
}