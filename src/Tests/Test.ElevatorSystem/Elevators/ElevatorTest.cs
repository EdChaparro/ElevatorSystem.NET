using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class ElevatorTest
    {
        [TestMethod]
        public void ShouldInstantiateFloorRequestPanel()
        {
            var e = new Elevator(1..2);
            Assert.IsNotNull(e.FloorRequestPanel);
        }

        [TestMethod]
        public void ShouldInstantiateFloorRequestPanelWithCorrectNumberOfButtons()
        {
            var e = new Elevator(3, 5, 7);
            Assert.AreEqual(3, e.FloorRequestPanel.NumberOfButtons);
        }

        [TestMethod]
        public void ShouldRaiseDoorEventWhenStatusChanges()
        {
            var elevator = new Elevator(1..2)
                { DoorStatus = DoorStatus.Closed };

            var receivedEvents = new List<ElevatorDoorEventArgs>();

            elevator.DoorStateChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            elevator.DoorStatus = DoorStatus.Closed; //No event generated; door already closed
            Assert.AreEqual(0, receivedEvents.Count);

            elevator.DoorStatus = DoorStatus.Open;
            Assert.AreEqual(1, receivedEvents.Count);

            var doorEvent = receivedEvents.First();

            Assert.AreEqual(DoorStatus.Open, doorEvent.DoorStatus);

            Assert.AreEqual(elevator.Id, doorEvent.ElevatorId);
        }

        [TestMethod]
        public void ShouldRaiseDirectionChangedEvent()
        {
            var elevator = new Elevator(1..5)
                { DoorStatus = DoorStatus.Closed };

            Assert.AreEqual(Direction.Up, elevator.Direction);

            var receivedEvents = new List<ElevatorDirectionChangedEventArgs>();

            elevator.DirectionChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            elevator.RequestStopAtFloorNumber(4);
            Assert.AreEqual(0, receivedEvents.Count);

            elevator.RequestStopAtFloorNumber(2);
            Assert.AreEqual(1, receivedEvents.Count);

            var directionEvent = receivedEvents.First();

            Assert.AreEqual(Direction.Down, directionEvent.Direction);

            Assert.AreEqual(elevator.Id, directionEvent.ElevatorId);
        }

        [TestMethod]
        public void ShouldRaiseFloorChangedEvent()
        {
            var elevator = new Elevator(1..2)
                { DoorStatus = DoorStatus.Open };

            Assert.AreEqual(1, elevator.CurrentFloorNumber);

            var receivedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            elevator.FloorNumberChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsFalse(elevator.RequestStopAtFloorNumber(1)); //No event generated; direction not changed
            Assert.AreEqual(0, receivedEvents.Count);

            Assert.IsTrue(elevator.RequestStopAtFloorNumber(2));
            Assert.AreEqual(1, receivedEvents.Count);

            var floorEvent = receivedEvents.First();

            Assert.AreEqual(2, floorEvent.CurrentFloorNbr);

            Assert.AreEqual(elevator.Id, floorEvent.ElevatorId);
        }

        [TestMethod]
        public void ShouldNotRaiseFloorChangedEventWhenDisabled()
        {
            var elevator = new Elevator(1..3)
            {
                DoorStatus = DoorStatus.Open,
            };

            Assert.AreEqual(1, elevator.CurrentFloorNumber);

            var receivedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            elevator.FloorNumberChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsTrue(elevator.RequestStopAtFloorNumber(2));
            Assert.AreEqual(1, receivedEvents.Count);

            elevator.IsEnabled = false;
            Assert.IsFalse(elevator.RequestStopAtFloorNumber(3));           //Additional event
            Assert.AreEqual(1, receivedEvents.Count);   // not raised
        }

        #region Updates Floor Request Buttons

        [TestMethod]
        public void ShouldResetFloorRequestButtonWhenDoorOpens()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            Assert.AreEqual(1, e.CurrentFloorNumber);
            var ePanel = e.FloorRequestPanel;
            var floor3RequestButton = ePanel.GetButtonForFloorNumber(3);
            Assert.IsFalse(floor3RequestButton.IsPressed);

            Assert.IsTrue(floor3RequestButton.SetPressedTo(true));
            Assert.IsTrue(floor3RequestButton.IsPressed);

            Assert.IsTrue(e.RequestStopAtFloorNumber(3));
            Assert.IsFalse(floor3RequestButton.IsPressed);
        }

        [TestMethod]
        public void ShouldIgnoreFloorRequestButtonWhenNotCongruentWithDirection()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            e.RequestStopAtFloorNumber(4);
            Assert.AreEqual(Direction.Up, e.Direction);

            var ePanel = e.FloorRequestPanel;
            var floor2RequestButton = ePanel.GetButtonForFloorNumber(2);
            Assert.IsFalse(floor2RequestButton.IsPressed);

            Assert.IsFalse(floor2RequestButton.SetPressedTo(true));
            Assert.IsFalse(floor2RequestButton.IsPressed);
        }
        #endregion

        [TestMethod]
        public void ShouldOnlyAcceptValidFloorNumbers()
        {
            var e = new Elevator(1..5);

            Assert.IsTrue(e.RequestStopAtFloorNumber(3));
            Assert.IsFalse(e.RequestStopAtFloorNumber(7));
        }

        [TestMethod]
        public void ShouldDefaultFloorNumberToLowestFloor()
        {
            var e = new Elevator(3..5);
            Assert.AreEqual(3, e.CurrentFloorNumber);
        }

        [TestMethod]
        public void ShouldTrackedRequestedFloorStops()
        {
            var e = new Elevator(1..10);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            var panel = e.FloorRequestPanel;

            Assert.IsTrue(panel.GetButtonForFloorNumber(2).SetPressedTo(true));
            Assert.IsTrue(panel.GetButtonForFloorNumber(4).SetPressedTo(true));
            Assert.IsTrue(panel.GetButtonForFloorNumber(6).SetPressedTo(true));
            Assert.IsTrue(panel.GetButtonForFloorNumber(8).SetPressedTo(true));

            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 }, e.RequestedFloorStops.ToList());
        }

        [TestMethod]
        public void ShouldIgnoreRedundantFloorStopRequests()
        {
            var e = new Elevator(1..10);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            var panel = e.FloorRequestPanel;

            Assert.IsTrue(panel.GetButtonForFloorNumber(2).SetPressedTo(true));
            Assert.IsFalse(panel.GetButtonForFloorNumber(2).SetPressedTo(true));
            Assert.IsTrue(panel.GetButtonForFloorNumber(5).SetPressedTo(true));
            Assert.IsFalse(panel.GetButtonForFloorNumber(5).SetPressedTo(true));

            CollectionAssert.AreEqual(new[] { 2, 5 }, e.RequestedFloorStops.ToList());
        }

        [TestMethod]
        public void ShouldUpdateRequestedFloorStopsListOnDoorOpen()
        {
            var e = new Elevator(1..7);
            var panel = e.FloorRequestPanel;

            Assert.IsFalse(panel.RequestedFloorStops.Any());

            Assert.IsTrue(panel.GetButtonForFloorNumber(4).SetPressedTo(true));
            Assert.IsTrue(panel.GetButtonForFloorNumber(7).SetPressedTo(true));

            CollectionAssert.AreEqual(new[] { 4, 7 }, e.RequestedFloorStops.ToList());

            Assert.IsTrue(e.RequestStopAtFloorNumber(4));
            CollectionAssert.AreEqual(new[] { 7 }, e.RequestedFloorStops.ToList());
        }

        [TestMethod]
        public void ShouldTraverseToFloorDestinationStepwise()
        {
            var elevator = new Elevator(1..9)
                { DoorStatus = DoorStatus.Closed };

            Assert.AreEqual(1, elevator.CurrentFloorNumber);

            var floorNumberChangedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            elevator.FloorNumberChangedEvent += (sender, e)
                => floorNumberChangedEvents.Add(e);

            Assert.IsTrue(elevator.RequestStopAtFloorNumber(5));
            Assert.AreEqual(4, floorNumberChangedEvents.Count);

            var floorNumber = 1;
            foreach (var e in floorNumberChangedEvents)
            {
                floorNumber++;
                Assert.AreEqual(floorNumber, e.CurrentFloorNbr);
            }

            //Door opens on arrival to floor destination
            Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
        }
    }
}