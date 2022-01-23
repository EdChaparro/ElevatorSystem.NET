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
            var e = new Elevator(1, 2);
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
            var elevator = new Elevator(1, 2)
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
            var elevator = new Elevator(1, 2)
                { DoorStatus = DoorStatus.Closed, Direction = Direction.Stationary };

            var receivedEvents = new List<ElevatorDirectionChangedEventArgs>();

            elevator.DirectionChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            elevator.Direction = Direction.Stationary; //No event generated; direction not changed
            Assert.AreEqual(0, receivedEvents.Count);

            elevator.Direction = Direction.Up;
            Assert.AreEqual(1, receivedEvents.Count);

            var directionEvent = receivedEvents.First();

            Assert.AreEqual(Direction.Up, directionEvent.Direction);

            Assert.AreEqual(elevator.Id, directionEvent.ElevatorId);
        }

        [TestMethod]
        public void ShouldRaiseFloorChangedEvent()
        {
            var elevator = new Elevator(1, 2)
                { DoorStatus = DoorStatus.Closed,
                    Direction = Direction.Stationary,
                    FloorNumber = null
                };

            var receivedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            elevator.FloorNumberChangedEvent += (sender, e)
                => receivedEvents.Add(e);

            elevator.FloorNumber = null; //No event generated; direction not changed
            Assert.AreEqual(0, receivedEvents.Count);

            elevator.FloorNumber = 2;
            Assert.AreEqual(1, receivedEvents.Count);

            var floorEvent = receivedEvents.First();

            Assert.AreEqual(2, floorEvent.CurrentFloorNbr);

            Assert.AreEqual(elevator.Id, floorEvent.ElevatorId);
        }
    }
}