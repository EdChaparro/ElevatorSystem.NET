using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class BankControllerTest
    {
        [TestMethod]
        public void ShouldTrackFloorElevatorCallRequests()
        {
            var bank = new Bank(2, 1..10);
            var controller = new BankController(bank);

            Assert.IsFalse(controller.RequestedFloorStopsDown.Any());
            Assert.IsFalse(controller.RequestedFloorStopsUp.Any());

            Assert.IsTrue(bank.PressButtonForFloorNumber(9, Direction.Down));
            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));

            Assert.AreEqual(1, controller.RequestedFloorStopsDown.Count());
            Assert.AreEqual(1, controller.RequestedFloorStopsUp.Count());

            Assert.AreEqual(9, controller.RequestedFloorStopsDown.First());
            Assert.AreEqual(5, controller.RequestedFloorStopsUp.First());
        }
    }
}