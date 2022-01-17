using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class FloorElevatorCallPanelTest
    {
        [TestMethod]
        public void ShouldAssignDownButton()
        {
            var floorPanel = new FloorElevatorCallPanel(true, false);

            Assert.IsNotNull(floorPanel.DownButton);
            Assert.AreEqual(Direction.Down, floorPanel.DownButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpButton()
        {
            var floorPanel = new FloorElevatorCallPanel(false, true);

            Assert.IsNotNull(floorPanel.UpButton);
            Assert.AreEqual(Direction.Up, floorPanel.UpButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpAndDownButton()
        {
            var floorPanel = new FloorElevatorCallPanel(true, true);

            Assert.IsNotNull(floorPanel.DownButton);
            Assert.IsNotNull(floorPanel.UpButton);
            Assert.AreEqual(Direction.Down, floorPanel.DownButton.Direction);
            Assert.AreEqual(Direction.Up, floorPanel.UpButton.Direction);
            Assert.AreEqual(2, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldIncludeAtLeastOneButton()
        {
            var floorPanel = new FloorElevatorCallPanel(false, false);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var panel = new FloorElevatorCallPanel(true, true);

            var receivedEvents =
                new List<PanelButtonPressedEventArgs<FloorElevatorCallButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            panel.DownButton.IsPressed = true;
            panel.UpButton.IsPressed = true;
            Assert.AreEqual(2, receivedEvents.Count);

            var firstEvent = receivedEvents.First();
            var downButton = firstEvent.GetButton<FloorElevatorCallButton>();

            var secondEvent = receivedEvents.Last();
            var upButton = secondEvent.GetButton<FloorElevatorCallButton>();

            Assert.AreEqual(Direction.Down, downButton.Direction);
            Assert.AreEqual(Direction.Up, upButton.Direction);
        }
    }
}