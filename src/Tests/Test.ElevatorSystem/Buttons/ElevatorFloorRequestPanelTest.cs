using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Tests.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class ElevatorFloorRequestPanelTest
    {
        [TestMethod]
        public void ShouldHaveButtonForEachFloor()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel
                (new Elevator(1, 2, 3));

            Assert.AreEqual(3, floorRequestPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldReturnCorrectFloorNumberButton()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel
                (new Elevator(1, 2, 3));

            var buttonForFloor1 = floorRequestPanel.GetButtonForFloorNumber(1);
            var buttonForFloor2 = floorRequestPanel.GetButtonForFloorNumber(2);
            var buttonForFloor3 = floorRequestPanel.GetButtonForFloorNumber(3);

            Assert.IsNotNull(buttonForFloor1);
            Assert.IsNotNull(buttonForFloor2);
            Assert.IsNotNull(buttonForFloor3);

            Assert.AreEqual(1, buttonForFloor1.FloorNbr);
            Assert.AreEqual(2, buttonForFloor2.FloorNbr);
            Assert.AreEqual(3, buttonForFloor3.FloorNbr);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRequireAtLeastTwoFloors()
        {
            new ElevatorFloorRequestPanel(new Elevator());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumberWhenZero()
        {
            new ElevatorFloorRequestPanel(new Elevator(0, 1, 2));
        }

        [TestMethod]
        public void ShouldProvidedRequestedFloorStops()
        {
            var elevator = new Elevator(1..7);
            var panel = elevator.FloorRequestPanel;

            Assert.IsFalse(panel.RequestedFloorStops.Any());

            Assert.IsTrue(elevator.PressButtonForFloorNumber(2));
            Assert.IsTrue(elevator.PressButtonForFloorNumber(4));
            Assert.IsTrue(elevator.PressButtonForFloorNumber(6));

            CollectionAssert.AreEqual(new[] { 2, 4, 6 }, panel.RequestedFloorStops.ToList());
        }
    }
}