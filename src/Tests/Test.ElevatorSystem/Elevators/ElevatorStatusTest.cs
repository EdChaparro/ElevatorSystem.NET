using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class ElevatorStatusTest
    {
        [TestMethod]
        public void ShouldReportDirectionCorrectly()
        {
            var eStatus = new ElevatorStatus();
            Assert.IsFalse(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Up);
            Assert.IsTrue(eStatus.IsMovingUp);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Down);
            Assert.IsTrue(eStatus.IsMovingDown);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Stationary);
            Assert.IsFalse(eStatus.IsMoving);
        }
    }
}