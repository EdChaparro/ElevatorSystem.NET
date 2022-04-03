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
        public void ShouldNotDoubleAssignElevators()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            Assert.IsTrue(bank.PressButtonForFloorNumber(8, Direction.Down));

            var strategy = new IdleStrategy();

            Assert.IsFalse(strategy.AssignedDownRequestedFloorStops.Any());
            strategy.AssignElevators(bank);
            Assert.AreEqual(1, strategy.AssignedDownRequestedFloorStops.Count());

            strategy.AssignElevators(bank); //Second call should have no effect
            Assert.AreEqual(1, strategy.AssignedDownRequestedFloorStops.Count());

            //TODO: Expand test to show new floor assignments will be honored.
        }

        //TODO: Create tests demonstrating Strategy cached is cleared when request honored
    }
}