using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests
{
    [TestClass]
    public class FloorTest
    {
        [TestMethod]
        public void ShouldDefaultNameToFloorNumber()
        {
            var floor = new Floor(5);

            Assert.AreEqual("5", floor.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumbersBelowOne()
        {
            var floor = new Floor(0);
        }

        [TestMethod]
        public void ShouldDefaultToUnlocked()
        {
            var floor = new Floor(5);

            Assert.IsFalse(floor.IsLocked);
        }

        [TestMethod]
        public void ShouldConsiderFloorsWithIdenticalNumbersEqual()
        {
            var floor1 = new Floor(5) { Name = "XXX" };
            var floor2 = new Floor(5) { Name = "YYY" };
            var floor3 = new Floor(3) { Name = "ABC" };

            Assert.AreEqual(floor1, floor2);
            Assert.AreNotEqual(floor1, floor3);
        }

        [TestMethod]
        public void ShouldSupportFloorOperatorComparisons()
        {
            var floor1 = new Floor(1) { Name = "XXX" };
            var floor2 = new Floor(2) { Name = "YYY" };

            Assert.IsTrue(floor1 < floor2);
            Assert.IsTrue(floor2 > floor1);
        }
    }
}