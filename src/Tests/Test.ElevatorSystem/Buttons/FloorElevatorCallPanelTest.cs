using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class FloorElevatorCallPanelTest
    {
        [TestMethod]
        public void ShouldAssignDownButton()
        {
            var floorPanel = new FloorElevatorCallPanel(2, true, false);

            Assert.IsNotNull(floorPanel.DownButton);
            Assert.AreEqual(Direction.Down, floorPanel.DownButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpButton()
        {
            var floorPanel = new FloorElevatorCallPanel(1, false, true);

            Assert.IsNotNull(floorPanel.UpButton);
            Assert.AreEqual(Direction.Up, floorPanel.UpButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpAndDownButton()
        {
            var floorPanel = new FloorElevatorCallPanel(2, true, true);

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
            new FloorElevatorCallPanel(1, false, false);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var panel = new FloorElevatorCallPanel(2, true, true);

            var receivedEvents =
                new List<PanelButtonPressedEventArgs<FloorElevatorCallButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsTrue(panel.DownButton.SetPressedTo(true));
            Assert.IsTrue(panel.UpButton.SetPressedTo(true));
            Assert.AreEqual(2, receivedEvents.Count);

            var firstEvent = receivedEvents.First();
            var downButton = firstEvent.Button;

            var secondEvent = receivedEvents.Last();
            var upButton = secondEvent.Button;

            Assert.AreEqual(Direction.Down, downButton.Direction);
            Assert.AreEqual(Direction.Up, upButton.Direction);
        }
    }
}