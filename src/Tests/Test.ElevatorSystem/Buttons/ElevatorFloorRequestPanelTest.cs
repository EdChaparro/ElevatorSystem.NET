using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Direction = IntrepidProducts.ElevatorSystem.Elevators.Direction;

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
        public void ShouldRaiseButtonPressedEvent()
        {
            var elevator = new Elevator(1, 2, 3);
            var panel = new ElevatorFloorRequestPanel(elevator);

            var receivedEvents =
                new List<PanelButtonPressedEventArgs<ElevatorFloorRequestButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            var buttonForFloor1 = panel.GetButtonForFloorNumber(1);
            var buttonForFloor2 = panel.GetButtonForFloorNumber(2);
            var buttonForFloor3 = panel.GetButtonForFloorNumber(3);

            Assert.IsTrue(buttonForFloor2.SetPressedTo(true));
            Assert.IsTrue(buttonForFloor3.SetPressedTo(true));
            Assert.AreEqual(2, receivedEvents.Count);

            var firstEvent = receivedEvents.First();
            var firstButton = firstEvent.GetButton<ElevatorFloorRequestButton>();

            var secondEvent = receivedEvents.Last();
            var secondButton = secondEvent.GetButton<ElevatorFloorRequestButton>();

            Assert.AreEqual(2, firstButton.FloorNbr);
            Assert.AreEqual(3, secondButton.FloorNbr);

            Assert.IsTrue(elevator.MoveToFloorNumber(3));
            elevator.Direction = Direction.Down;
            Assert.IsTrue(buttonForFloor1.SetPressedTo(true));
            Assert.AreEqual(3, receivedEvents.Count);
            var thirdEvent = receivedEvents.Last();
            var thirdButton = thirdEvent.GetButton<ElevatorFloorRequestButton>();
            Assert.AreEqual(1, thirdButton.FloorNbr);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRequireAtLeastTwoFloors()
        {
            new ElevatorFloorRequestPanel(new Elevator());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumbersLessThanOne()
        {
            new ElevatorFloorRequestPanel(new Elevator(0, 1, 2));
        }
    }
}