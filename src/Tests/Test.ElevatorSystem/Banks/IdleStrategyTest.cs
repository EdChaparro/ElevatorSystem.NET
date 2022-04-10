using System.Linq;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
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
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr),
                Direction.Down).ToList();

            Assert.AreEqual(1, assignments.Count());
            var rfs = assignments.First();

            Assert.AreEqual(1, elevator1.RequestedFloorStops.Count());
            Assert.AreEqual(rfs, elevator1.RequestedFloorStops.First());
        }
    }
}