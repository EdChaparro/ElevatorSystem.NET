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
            var bank = new Bank(2, new Floor(1), new Floor(2));
            var floorPanel = new FloorElevatorCallPanel(bank, true, false);

            Assert.IsNotNull(floorPanel.DownButton);
            Assert.AreEqual(Direction.Down, floorPanel.DownButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpButton()
        {
            var bank = new Bank(2, new Floor(1), new Floor(2));
            var floorPanel = new FloorElevatorCallPanel(bank, false, true);

            Assert.IsNotNull(floorPanel.UpButton);
            Assert.AreEqual(Direction.Up, floorPanel.UpButton.Direction);
            Assert.AreEqual(1, floorPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldAssignUpAndDownButton()
        {
            var bank = new Bank(2, new Floor(1), new Floor(2));
            var floorPanel = new FloorElevatorCallPanel(bank, true, true);

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
            var bank = new Bank(2, new Floor(1), new Floor(2));
            new FloorElevatorCallPanel(bank, false, false);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var bank = new Bank(2, new Floor(1), new Floor(2));
            var panel = new FloorElevatorCallPanel(bank, true, true);

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