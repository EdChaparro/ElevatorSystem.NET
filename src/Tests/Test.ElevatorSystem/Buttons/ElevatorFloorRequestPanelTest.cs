using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var panel = new ElevatorFloorRequestPanel
                (new Elevator(1, 2, 3));

            var receivedEvents =
                new List<PanelButtonPressedEventArgs<ElevatorFloorRequestButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            var buttonForFloor1 = panel.GetButtonForFloorNumber(1);
            var buttonForFloor2 = panel.GetButtonForFloorNumber(2);
            var buttonForFloor3 = panel.GetButtonForFloorNumber(3);

            buttonForFloor2.IsPressed = true;
            buttonForFloor3.IsPressed = true;
            Assert.AreEqual(2, receivedEvents.Count);

            var firstEvent = receivedEvents.First();
            var firstButton = firstEvent.GetButton<ElevatorFloorRequestButton>();

            var secondEvent = receivedEvents.Last();
            var secondButton = secondEvent.GetButton<ElevatorFloorRequestButton>();

            Assert.AreEqual(2, firstButton.FloorNbr);
            Assert.AreEqual(3, secondButton.FloorNbr);

            buttonForFloor1.IsPressed = true;
            Assert.AreEqual(3, receivedEvents.Count);
            var thirdEvent = receivedEvents.Last();
            var thirdButton = thirdEvent.GetButton<ElevatorFloorRequestButton>();
            Assert.AreEqual(1, thirdButton.FloorNbr);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRequreAtLeastTwoFloors()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel(new Elevator());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumbersLessThanOne()
        {
            var floorRequestPanel = new ElevatorFloorRequestPanel
                (new Elevator(0, 1, 2));
        }
    }
}