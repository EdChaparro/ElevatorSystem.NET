using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class ElevatorTest
    {
        public ElevatorTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

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
        public void ShouldOnlyAcceptValidFloorNumbers()
        {
            var e = new Elevator(1..5);

            Assert.IsTrue(e.RequestStopAtFloorNumber(3).isOk);
            Assert.IsFalse(e.RequestStopAtFloorNumber(7).isOk);
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

            Assert.IsTrue(e.PressButtonForFloorNumber(2));
            Assert.IsTrue(e.PressButtonForFloorNumber(4));
            Assert.IsTrue(e.PressButtonForFloorNumber(6));
            Assert.IsTrue(e.PressButtonForFloorNumber(8));

            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 },
                e.RequestedFloorStops.Select(x => x.FloorNbr).ToList());
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

        [TestMethod]
        public void ShouldNeverReportIdleWhenDisabled()
        {
            var e = new Elevator(1..9);
            Assert.IsTrue(e.IsIdle);

            e.IsEnabled = false;
            Assert.IsFalse(e.IsIdle);
        }

        [TestMethod]
        public void ShouldSetCorrectRequestFloorStopDirection()
        {
            var e = new Elevator(1..9);

            var rfs1 = e.RequestStopAtFloorNumber(7).requestedFloorStop;
            Assert.IsNotNull(rfs1);
            Assert.AreEqual(Direction.Up, rfs1.Direction); //Defaults to heading

            var rfs2 = e.RequestStopAtFloorNumber(8, Direction.Down).requestedFloorStop;
            Assert.IsNotNull(rfs2);
            Assert.AreEqual(Direction.Down, rfs2.Direction); //Can be overridden
        }

        public static void WaitForElevatorToReachFloor
            (int floorNbr, Elevator elevator, int timeOutInSeconds = 10)
        {
            if ((elevator.CurrentFloorNumber == floorNbr) &&
                (elevator.DoorStatus == DoorStatus.Open))
            {
                return;     //All done
            }

            var isFloorReached = false;

            elevator.DoorStateChangedEvent += (sender, e)
                =>
            {
                if (elevator.DoorStatus == DoorStatus.Closed)
                {
                    return;
                }

                if (elevator.CurrentFloorNumber == floorNbr)
                {
                    isFloorReached = true;
                }
            };

            var timeOut = DateTime.Now + new TimeSpan(0, 0, timeOutInSeconds);
            var isTimeOut = false;

            while (!isTimeOut)
            {
                if (isFloorReached)
                {
                    break;
                }

                if (DateTime.Now > timeOut)
                {
                    isTimeOut = true;
                }

                Thread.Sleep(500);
            }

            Assert.IsFalse(isTimeOut);

            //Added to resolved test flakiness. Some Elevator Events were
            //unpredictably firing after test assertions, leading to random failures.
            Thread.Sleep(100);
        }
    }
}