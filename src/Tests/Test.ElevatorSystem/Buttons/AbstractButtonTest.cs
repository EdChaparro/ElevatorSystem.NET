using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    [TestClass]
    public class AbstractButtonTest
    {
        [TestMethod]
        public void ShouldInitializeEnabled()
        {
            Assert.IsTrue(new TestButton().IsEnabled);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var button = new TestButton();

            var receivedEvents = new List<ButtonPressedEventArgs<IButton>>();

            button.ButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsFalse(button.SetPressedTo(false));  //No event generated for false
            Assert.AreEqual(0, receivedEvents.Count);

            Assert.IsTrue(button.SetPressedTo(true));    //Expect event to be generated
            Assert.AreEqual(1, receivedEvents.Count);

            var eventButton = receivedEvents.First().Button as TestButton;
            Assert.IsNotNull(eventButton);

            Assert.AreEqual(button.Id, eventButton.Id);
        }

        [TestMethod]
        public void ShouldNotRaiseButtonPressedEventWhenAlreadyPressed()
        {
            var button = new TestButton();

            var receivedEvents = new List<ButtonPressedEventArgs<IButton>> ();

            button.ButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsTrue(button.SetPressedTo(true));   //Expect event to be generated
            Assert.AreEqual(1, receivedEvents.Count);

            Assert.IsFalse(button.SetPressedTo(true));  //Redundant press event doesn't generate event
            Assert.AreEqual(1, receivedEvents.Count);

            Assert.IsTrue(button.SetPressedTo(false));  //reset
            Assert.IsTrue(button.SetPressedTo(true));   //This should generate a new event
            Assert.AreEqual(2, receivedEvents.Count);
        }

        [TestMethod]
        public void ShouldNotRaiseButtonPressedEventWhenDisabled()
        {
            var button = new TestButton();

            ButtonPressedEventArgs<IButton> receivedEvent = null;

            button.ButtonPressedEvent += (sender, e)
                => receivedEvent = e;

            button.IsEnabled = false;
            Assert.IsFalse(button.SetPressedTo(true));
            
            Assert.IsNull(receivedEvent);
        }

        [TestMethod]
        public void ShouldIncludeButtonInRaiseButtonPressedEvent()
        {
            var button = new TestButton();

            ButtonPressedEventArgs<IButton> receivedEvent = null;

            button.ButtonPressedEvent += (sender, e)
                => receivedEvent = e;

            Assert.IsTrue(button.SetPressedTo(true));   //This should generate an event

            var eventButton = receivedEvent.Button as TestButton;
            Assert.IsNotNull(eventButton);
            Assert.AreEqual(button.Id, eventButton.Id);
        }

        [TestMethod]
        public void ShouldResetButtonIsPressedStateWhenDisabled()
        {
            var button = new TestButton();

            Assert.IsTrue(button.IsEnabled);
            Assert.IsTrue(button.SetPressedTo(true));
            Assert.IsTrue(button.IsPressed);

            button.IsEnabled = false;
            Assert.IsFalse(button.IsPressed);
        }
    }
}
