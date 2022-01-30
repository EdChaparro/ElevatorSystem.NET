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
            var eStatus = new ElevatorStatus(Direction.Up, DoorStatus.Open);
            Assert.IsFalse(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Up, DoorStatus.Closed);
            Assert.IsTrue(eStatus.IsMovingUp);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Down, DoorStatus.Closed);
            Assert.IsTrue(eStatus.IsMovingDown);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(Direction.Up, DoorStatus.Open);
            Assert.IsFalse(eStatus.IsMoving);
        }
    }
}