using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public void ShouldSyncFloorRequestPanelState()
        {
            var e = new Elevator(1..3);
            Assert.IsTrue(e.IsEnabled);
            Assert.AreEqual(e.IsEnabled, e.FloorRequestPanel.IsEnabled);

            e.IsEnabled = false;
            Assert.IsFalse(e.IsEnabled);
            Assert.AreEqual(e.IsEnabled, e.FloorRequestPanel.IsEnabled);
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

            elevator.Start();
            elevator.RequestStopAtFloorNumber(4);
            Assert.AreEqual(0, receivedEvents.Count);
            WaitForElevatorToReachFloor(4, elevator);

            elevator.RequestStopAtFloorNumber(2);
            WaitForElevatorToReachFloor(2, elevator);
            Assert.AreEqual(1, receivedEvents.Count);

            var directionEvent = receivedEvents.First();

            Assert.AreEqual(Direction.Down, directionEvent.Direction);
            Assert.AreEqual(elevator.Id, directionEvent.ElevatorId);
            elevator.Stop();
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

            elevator.Start();
            Assert.IsFalse(elevator.RequestStopAtFloorNumber(1)); //No event generated;
            Assert.AreEqual(0, receivedEvents.Count);   //already at 1st floor

            Assert.IsTrue(elevator.RequestStopAtFloorNumber(2));
            WaitForElevatorToReachFloor(2, elevator);
            Assert.AreEqual(1, receivedEvents.Count);

            var floorEvent = receivedEvents.First();

            Assert.AreEqual(2, floorEvent.CurrentFloorNbr);
            Assert.AreEqual(elevator.Id, floorEvent.ElevatorId);

            elevator.Stop();
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

            elevator.Start();
            Assert.IsTrue(elevator.RequestStopAtFloorNumber(2));
            WaitForElevatorToReachFloor(2, elevator);
            Assert.AreEqual(1, receivedEvents.Count);

            elevator.IsEnabled = false;
            Assert.IsFalse(elevator.RequestStopAtFloorNumber(3));  //Additional event
            Assert.AreEqual(1, receivedEvents.Count);   // not raised
            elevator.Stop();
        }

        #region Updates Floor Request Buttons

        [TestMethod]
        public void ShouldResetFloorRequestButtonWhenDoorOpens()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            e.Start();
            Assert.AreEqual(1, e.CurrentFloorNumber);
            var ePanel = e.FloorRequestPanel;
            var floor3RequestButton = ePanel.GetButtonForFloorNumber(3);
            Assert.IsFalse(floor3RequestButton.IsPressed);

            var floorNumberChangedEventCount = 0;
            e.FloorNumberChangedEvent += (sender, e)
                =>
            {
                floorNumberChangedEventCount++;
                Assert.IsTrue(floor3RequestButton.IsPressed);
            };

            Assert.IsTrue(floor3RequestButton.SetPressedTo(true));
            WaitForElevatorToReachFloor(3, e);
            Assert.AreEqual(2, floorNumberChangedEventCount); //Confirm we got expected events
            Assert.IsFalse(floor3RequestButton.IsPressed);
            e.Stop();
        }

        [TestMethod]
        public void ShouldIgnoreFloorRequestButtonWhenNotCongruentWithDirection()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            e.Start();
            e.RequestStopAtFloorNumber(4);
            Assert.AreEqual(Direction.Up, e.Direction);
            WaitForElevatorToReachFloor(4, e);

            var ePanel = e.FloorRequestPanel;
            var floor2RequestButton = ePanel.GetButtonForFloorNumber(2);
            Assert.IsFalse(floor2RequestButton.IsPressed);

            Assert.IsFalse(floor2RequestButton.SetPressedTo(true));
            Assert.IsFalse(floor2RequestButton.IsPressed);

            e.Stop();
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

        [TestMethod, Ignore] //TODO: Set never has more than one due to immediate movement, fix?
        public void ShouldTrackedRequestedFloorStops()
        {
            var e = new Elevator(1..10);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            Assert.IsTrue(e.PressButtonForFloorNumber(2));
            Assert.IsTrue(e.PressButtonForFloorNumber(4));
            Assert.IsTrue(e.PressButtonForFloorNumber(6));
            Assert.IsTrue(e.PressButtonForFloorNumber(8));

            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 }, e.RequestedFloorStops.ToList());
        }

        [TestMethod]
        public void ShouldIgnoreRedundantFloorStopRequests()
        {
            var e = new Elevator(1..10);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            Assert.IsTrue(e.PressButtonForFloorNumber(2));
            Assert.IsFalse(e.PressButtonForFloorNumber(2));
            Assert.IsTrue(e.PressButtonForFloorNumber(5));
            Assert.IsFalse(e.PressButtonForFloorNumber(5));
        }

        [TestMethod, Ignore] //TODO: Set never has more than one due to immediate movement, fix?
        public void ShouldUpdateRequestedFloorStopsListOnDoorOpen()
        {
            var e = new Elevator(1..7);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            Assert.IsTrue(e.PressButtonForFloorNumber(4));
            Assert.IsTrue(e.PressButtonForFloorNumber(7));

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

            elevator.Start();
            Assert.IsTrue(elevator.RequestStopAtFloorNumber(5));
            WaitForElevatorToReachFloor(5, elevator);
            Assert.AreEqual(4, floorNumberChangedEvents.Count);

            var floorNumber = 1;
            foreach (var e in floorNumberChangedEvents)
            {
                floorNumber++;
                Assert.AreEqual(floorNumber, e.CurrentFloorNbr);
            }

            //Door opens on arrival to floor destination
            Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
            elevator.Stop();
        }

        [TestMethod]
        public void ShouldStopAtFloorWhenRequestedFromFloorPanel()
        {
            var elevator = new Elevator(1..9)
                { DoorStatus = DoorStatus.Closed };

            Assert.AreEqual(1, elevator.CurrentFloorNumber);

            elevator.Start();
            Assert.IsTrue(elevator.PressButtonForFloorNumber(5));
            WaitForElevatorToReachFloor(5, elevator);
            Assert.AreEqual(5, elevator.CurrentFloorNumber);
            elevator.Stop();
        }

        public static void WaitForElevatorToReachFloor
            (int floorNbr, Elevator elevator, int timeOutInSeconds = 10)
        {
            var timeOut = DateTime.Now + new TimeSpan(0, 0, timeOutInSeconds);
            var isTimeOut = false;

            while (!isTimeOut)
            {
                if (elevator.CurrentFloorNumber == floorNbr)
                {
                    break;
                }

                if (DateTime.Now > timeOut)
                {
                    isTimeOut = true;
                }

                Thread.Sleep(1000);
            }

            Assert.IsFalse(isTimeOut);

            //Added to resolved test flakiness. Some Elevator Events were
            //unpredictably firing after test assertions, leading to random failures.
            Thread.Sleep(100);
        }
    }
}