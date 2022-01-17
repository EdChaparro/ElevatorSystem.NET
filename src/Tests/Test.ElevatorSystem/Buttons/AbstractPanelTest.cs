using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            button2.IsPressed = true;   
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

            button2.IsPressed = true;
            Assert.AreEqual(0, receivedEvents.Count);

            panel.IsEnabled = true;
            button2.IsPressed = false;  //reset button
            button2.IsPressed = true;
            Assert.AreEqual(1, receivedEvents.Count);
        }
    }
}