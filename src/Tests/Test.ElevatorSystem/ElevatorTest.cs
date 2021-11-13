using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Tests
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