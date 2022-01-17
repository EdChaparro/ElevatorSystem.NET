using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class ElevatorFloorRequestPanelTest
    {
        [TestMethod]
        public void ShouldHaveButtonForEachFloor()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel(1, 2, 3);

            Assert.AreEqual(3, floorRequestPanel.NumberOfButtons);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRequreAtLeastTwoFloors()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumbersLessThanOne()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel(0, 1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRequireUniqueFloorNumbers()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel(1, 2, 2, 3);
        }
    }
}