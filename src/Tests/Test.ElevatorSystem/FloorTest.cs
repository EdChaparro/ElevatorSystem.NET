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

    }
}
