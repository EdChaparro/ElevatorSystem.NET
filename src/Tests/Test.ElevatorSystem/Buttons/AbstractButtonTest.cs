using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    public class TestButton : AbstractButton
    {
        public Guid Id = new Guid();
    }

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

            var receivedEvents = new List<ButtonPressedEventArgs>();

            button.ButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            button.IsPressed = false;   //No event generated for false
            Assert.AreEqual(0, receivedEvents.Count);

            button.IsPressed = true;    //Expect event to be generated
            Assert.AreEqual(1, receivedEvents.Count);

            var eventButton = receivedEvents.First().Button as TestButton;
            Assert.IsNotNull(eventButton);

            Assert.AreEqual(button.Id, eventButton.Id);
        }

        [TestMethod]
        public void ShouldNotRaiseButtonPressedEventWhenAlreadyPressed()
        {
            var button = new TestButton();

            var receivedEvents = new List<ButtonPressedEventArgs>();

            button.ButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            button.IsPressed = true;    //Expect event to be generated
            Assert.AreEqual(1, receivedEvents.Count);

            button.IsPressed = true;    //Redundant press event doesn't generate event
            Assert.AreEqual(1, receivedEvents.Count);

            button.IsPressed = false;   //reset
            button.IsPressed = true;    //This should generate a new event
            Assert.AreEqual(2, receivedEvents.Count);
        }

        [TestMethod]
        public void ShouldNotRaiseButtonPressedEventWhenDisabled()
        {
            var button = new TestButton();

            ButtonPressedEventArgs receivedEvent = null;

            button.ButtonPressedEvent += (sender, e)
                => receivedEvent = e;

            button.IsEnabled = false;
            button.IsPressed = true; 
            
            Assert.IsNull(receivedEvent);
        }

        [TestMethod]
        public void ShouldIncludeButtonInRaiseButtonPressedEvent()
        {
            var button = new TestButton();

            ButtonPressedEventArgs receivedEvent = null;

            button.ButtonPressedEvent += (sender, e)
                => receivedEvent = e;

            button.IsPressed = true;    //This should generate an event

            var eventButton = receivedEvent.Button as TestButton;
            Assert.IsNotNull(eventButton);
            Assert.AreEqual(button.Id, eventButton.Id);
        }
    }
}
