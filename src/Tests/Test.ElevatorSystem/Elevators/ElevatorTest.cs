using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class ElevatorTest
    {
        [TestMethod]
        public void ShouldReportDirectionCorrectly()
        {
            var elevator = new Elevator();
            Assert.IsFalse(elevator.IsMoving);

            elevator.Direction = Direction.Up;
            Assert.IsTrue(elevator.IsMovingUp);
            Assert.IsTrue(elevator.IsMoving);

            elevator.Direction = Direction.Down;
            Assert.IsTrue(elevator.IsMovingDown);
            Assert.IsTrue(elevator.IsMoving);

            elevator.Direction = Direction.Stationary;
            Assert.IsFalse(elevator.IsMoving);
        }
    }
}