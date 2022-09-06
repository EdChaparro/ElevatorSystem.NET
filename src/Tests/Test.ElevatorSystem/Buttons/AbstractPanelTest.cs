using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class AbstractPanelTest
    {
        [TestMethod]
        public void ShouldKeepButtonCount()
        {
            var panel = new TestPanel();
            Assert.AreEqual(0, panel.NumberOfButtons);

            panel.AddButton(new TestButton(), new TestButton());
            Assert.AreEqual(2, panel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var panel = new TestPanel();

            var receivedEvents = new List<PanelButtonPressedEventArgs<TestButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            var button1 = new TestButton();
            var button2 = new TestButton();
            var button3 = new TestButton();

            panel.AddButton(button1, button2, button3);

            Assert.IsTrue(button2.SetPressedTo(true));
            Assert.AreEqual(1, receivedEvents.Count);

            var e = receivedEvents.First();
            Assert.AreEqual(panel.Id, e.GetPanel<TestPanel>().Id);
        }

        [TestMethod]
        public void ShouldNotRaiseButtonPressedEventWhenDisabled()
        {
            var panel = new TestPanel { IsEnabled = false };

            var receivedEvents = new List<PanelButtonPressedEventArgs<TestButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            var button1 = new TestButton();
            var button2 = new TestButton();
            var button3 = new TestButton();

            panel.AddButton(button1, button2, button3);

            Assert.IsFalse(button2.SetPressedTo(true)); //False because panel is disabled
            Assert.AreEqual(0, receivedEvents.Count);

            panel.IsEnabled = true;
            Assert.IsTrue(button2.SetPressedTo(true));
            Assert.AreEqual(1, receivedEvents.Count);
        }

        [TestMethod]
        public void ShouldSetButtonEnabledStateToMatchPanel()
        {
            var panel = new TestPanel();

            var button1 = new TestButton();
            var button2 = new TestButton();
            var button3 = new TestButton();
            panel.AddButton(button1, button2, button3);

            Assert.IsTrue(button1.IsEnabled);
            Assert.IsTrue(button2.IsEnabled);
            Assert.IsTrue(button3.IsEnabled);

            panel.IsEnabled = false;
            Assert.IsFalse(button1.IsEnabled);
            Assert.IsFalse(button2.IsEnabled);
            Assert.IsFalse(button3.IsEnabled);

            panel.IsEnabled = true;
            Assert.IsTrue(button1.IsEnabled);
            Assert.IsTrue(button2.IsEnabled);
            Assert.IsTrue(button3.IsEnabled);
        }

        [TestMethod]
        public void ShouldResetButtonIsPressedStateWhenDisabled()
        {
            var panel = new TestPanel();

            var button1 = new TestButton();
            var button2 = new TestButton();
            var button3 = new TestButton();
            panel.AddButton(button1, button2, button3);

            Assert.IsTrue(button1.SetPressedTo(true));
            Assert.IsTrue(button2.SetPressedTo(true));
            Assert.IsTrue(button3.SetPressedTo(true));

            Assert.IsTrue(button1.IsPressed);
            Assert.IsTrue(button2.IsPressed);
            Assert.IsTrue(button3.IsPressed);

            panel.IsEnabled = false;
            Assert.IsFalse(button1.IsPressed);
            Assert.IsFalse(button2.IsPressed);
            Assert.IsFalse(button3.IsPressed);
        }

        [TestMethod]
        public void ShouldReportWhichButtonsArePressed()
        {
            var panel = new TestPanel();

            var button1 = new TestButton();
            var button2 = new TestButton();
            var button3 = new TestButton();
            panel.AddButton(button1, button2, button3);

            Assert.IsTrue(button1.SetPressedTo(true));
            Assert.IsTrue(button3.SetPressedTo(true));

            var pressedButtons = panel.PressedButtons.ToList();
            Assert.AreEqual(2, pressedButtons.Count());

            CollectionAssert.AreEquivalent
                (new TestButton[] { button1, button3 }, pressedButtons);
        }
    }
}