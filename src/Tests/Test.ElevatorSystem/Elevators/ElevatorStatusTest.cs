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
            var eStatus = new ElevatorStatus(1, Direction.Up, DoorStatus.Open);
            Assert.IsFalse(eStatus.IsMoving);

            eStatus = new ElevatorStatus(1, Direction.Up, DoorStatus.Closed);
            Assert.IsTrue(eStatus.IsMovingUp);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(5, Direction.Down, DoorStatus.Closed);
            Assert.IsTrue(eStatus.IsMovingDown);
            Assert.IsTrue(eStatus.IsMoving);

            eStatus = new ElevatorStatus(1, Direction.Up, DoorStatus.Open);
            Assert.IsFalse(eStatus.IsMoving);
        }
    }
}