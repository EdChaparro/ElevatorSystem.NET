using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Tests.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
{
    [TestClass]
    public class BankTest
    {
        public BankTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

        [TestMethod]
        public void ShouldSendElevatorsToLowestFloorWithDoorOpenOnStart()
        {
            var bank = new Bank(2, 1..5);

            bank.Start();
            var elevators = bank.Elevators.ToList();
            Assert.AreEqual(2, elevators.Count());

            foreach (var elevator in elevators)
            {
                Assert.AreEqual(bank.LowestFloorNbr, elevator.CurrentFloorNumber);
                Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
            }

            bank.Stop();
        }

        #region Elevators

        [TestMethod]
        public void ShouldKeepElevatorCount()
        {
            var bank = new Bank(2, 1..2);

            Assert.AreEqual(2, bank.NumberOfElevators);
        }

        [TestMethod]
        public void ShouldTrackFloorElevatorCallRequests()
        {
            var bank = new Bank(2, 1..10);

            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Down).Any());
            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Up).Any());

            Assert.IsTrue(bank.PressButtonForFloorNumber(9, Direction.Down));
            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));

            Assert.AreEqual(1, bank.GetRequestedFloorStops(Direction.Down).Count());
            Assert.AreEqual(1, bank.GetRequestedFloorStops(Direction.Up).Count());

            Assert.AreEqual(9, bank.GetRequestedFloorStops(Direction.Down)
                .Select(x => x.FloorNbr).First());
            Assert.AreEqual(5, bank.GetRequestedFloorStops(Direction.Up)
                .Select(x => x.FloorNbr).First());
        }

        [TestMethod]
        public void ShouldUpdateRequestedFloorStopOnElevatorArrival()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));
            Assert.IsTrue(bank.PressButtonForFloorNumber(9, Direction.Down));

            CollectionAssert.AreEqual(new[] { 5 },
                bank.GetRequestedFloorStops(Direction.Up).Select(x => x.FloorNbr).ToList());

            CollectionAssert.AreEqual(new[] { 9 },
                bank.GetRequestedFloorStops(Direction.Down).Select(x => x.FloorNbr).ToList());

            elevator1.Start();
            elevator1.RequestStopAtFloorNumber(5);
            ElevatorTest.WaitForElevatorToReachFloor(5, elevator1);
            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Up).Any());
            elevator1.Stop();

            elevator2.Start();
            elevator2.RequestStopAtFloorNumber(9);
            ElevatorTest.WaitForElevatorToReachFloor(9, elevator2, 20);
            Assert.AreEqual(Direction.Down, elevator2.Direction);
            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Down).Any());
            elevator2.Stop();
        }

        [TestMethod]
        public void ShouldTrackAssignedElevators()
        {
            var bank = new Bank(2, 1..50);
            var elevator = bank.Elevators.First(); //First idle elevator will be assigned

            Assert.IsFalse(bank.AssignedFloorStops.Any());

            bank.Start();
            Assert.IsTrue(bank.PressButtonForFloorNumber(14, Direction.Down));

            Thread.Sleep(500);  //Give the Engine a chance to do its thing

            Assert.IsTrue(bank.AssignedFloorStops.Any());

            ElevatorTest.WaitForElevatorToReachFloor(14, elevator);

            Assert.IsFalse(bank.AssignedFloorStops.Any());   //Cache should be clear

            bank.Stop();
        }

        [TestMethod]
        public void ShouldChangeElevatorDirectionWhenIdle()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            bank.Start();
            Assert.AreEqual(Direction.Up, elevator1.Direction);
            Assert.AreEqual(Direction.Up, elevator2.Direction);

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Down));

            //First idle elevator will be selected by Bank Engine
            ElevatorTest.WaitForElevatorToReachFloor(5, elevator1);

            Assert.IsTrue(elevator1.IsIdle);  //Should change direction when elevator idle
            Assert.AreEqual(Direction.Down, elevator1.Direction);
            Assert.AreEqual(5, elevator1.CurrentFloorNumber);
            Assert.IsTrue(elevator1.PressButtonForFloorNumber(1));  //Send it back down

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));
            //Expecting second elevator to respond to Floor Up Call Request
            ElevatorTest.WaitForElevatorToReachFloor(5, elevator2);
            Assert.IsTrue(elevator2.IsIdle);  //Should change direction when elevator idle
            Assert.AreEqual(Direction.Up, elevator2.Direction);
            Assert.AreEqual(5, elevator2.CurrentFloorNumber);

            bank.Stop();
        }

        [TestMethod]
        public void ShouldTrackPendingElevatorStopsGoingUp()
        {
            var bank = new Bank(3, 1..10);
            var elevator1 = bank.Elevators.ElementAt(0);
            var elevator2 = bank.Elevators.ElementAt(1);
            var elevator3 = bank.Elevators.ElementAt(2);

            elevator1.Start();
            elevator2.Start();
            elevator3.Start();

            Assert.IsTrue(elevator3.RequestStopAtFloorNumber(9).isOk);
            ElevatorTest.WaitForElevatorToReachFloor(9, elevator3);

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(3).isOk);   //Going up
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(7).isOk);   //Going up
            Assert.IsTrue(elevator3.RequestStopAtFloorNumber(1).isOk);   //Going down

            CollectionAssert.AreEqual(new[] { 3, 7 },
                bank.PendingUpFloorStops.Select(x => x.FloorNbr).ToList());

            elevator1.Stop();
            elevator2.Stop();
            elevator3.Stop();
        }

        [TestMethod]
        public void ShouldTrackPendingElevatorStopsGoingDown()
        {
            var bank = new Bank(3, 1..10);
            var elevator1 = bank.Elevators.ElementAt(0);
            var elevator2 = bank.Elevators.ElementAt(1);
            var elevator3 = bank.Elevators.ElementAt(2);

            elevator1.Start();
            elevator2.Start();
            elevator3.Start();

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(8).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(9).isOk);
            ElevatorTest.WaitForElevatorToReachFloor(9, elevator2);

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(2).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(3).isOk);
            Assert.IsTrue(elevator3.RequestStopAtFloorNumber(5).isOk);   //Is going Up

            Assert.AreEqual(2, bank.PendingDownFloorStops.Count());

            CollectionAssert.AreEqual(new[] { 2, 3 },
                bank.PendingDownFloorStops.Select(x => x.FloorNbr).ToList());

            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(2, Direction.Down));
            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(3, Direction.Down));
            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(5, Direction.Up));

            elevator1.Stop();
            elevator2.Stop();
            elevator3.Stop();
        }

        #endregion

        #region Floors

        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var bankWithFloors = new Bank(3, 1..2);
            Assert.AreEqual(2, bankWithFloors.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportLowestFloorNumber()
        {
            var bankWithFloors = new Bank(3, 1, 7, 5);
            Assert.AreEqual(1, bankWithFloors.LowestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportHighestFloorNumber()
        {
            var bank = new Bank(2, 1, 7, 5);
            Assert.AreEqual(7, bank.HighestFloorNbr);
        }

        [TestMethod]
        public void ShouldAcceptNegativeFloorNumbers()
        {
            var bank = new Bank(2, -3, -2, -1, 1, 2);
            Assert.AreEqual(5, bank.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportOrderedCollectionOfFloorNumbers()
        {
            var bank = new Bank(1, 1, 7, 5);

            var expectedFloorList = new[] { 1, 5, 7 };
            Assert.IsTrue(expectedFloorList.SequenceEqual(bank.OrderedFloorNumbers));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueFloors()
        {
            new Bank(3, 1, 2, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectFloorNumberZero()
        {
            new Bank(3, -1, 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAcceptBanksWithLessThanTwoFloors()
        {
            new Bank(1, 1);
        }

        [TestMethod]
        public void ShouldOnlyIncludeAppropriateCallButtonForEachFloor()
        {
            var bank = new Bank(2, 1..3);

            var floor1 = bank.GetFloorFor(1);
            var floor2 = bank.GetFloorFor(2);
            var floor3 = bank.GetFloorFor(3);

            Assert.IsNotNull(floor1.Panel);
            Assert.IsNotNull(floor2.Panel);
            Assert.IsNotNull(floor3.Panel);

            Assert.IsNotNull(floor1.Panel.UpButton);
            Assert.IsNull(floor1.Panel.DownButton);

            Assert.IsNotNull(floor2.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);

            Assert.IsNull(floor3.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);
        }

        [TestMethod]
        public void ShouldUseFloorNumberToDetermineRequiredCallButtons()
        {
            var bank = new Bank(2, 2, 3, 1); //Order doesn't matter

            var floor1 = bank.GetFloorFor(1);
            var floor2 = bank.GetFloorFor(2);
            var floor3 = bank.GetFloorFor(3);

            Assert.IsNotNull(floor1.Panel);
            Assert.IsNotNull(floor2.Panel);
            Assert.IsNotNull(floor3.Panel);

            Assert.IsNotNull(floor1.Panel.UpButton);
            Assert.IsNull(floor1.Panel.DownButton);

            Assert.IsNotNull(floor2.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);

            Assert.IsNull(floor3.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);
        }

        [TestMethod]
        public void ShouldProvideElevatorCallPanelForAllFloors()
        {
            var bank = new Bank(2, 1..3);

            var floor1Panel = bank.GetFloorElevatorCallPanelFor(1);
            Assert.IsNotNull(floor1Panel);

            var floor2Panel = bank.GetFloorElevatorCallPanelFor(2);
            Assert.IsNotNull(floor2Panel);

            var floor3Panel = bank.GetFloorElevatorCallPanelFor(3);
            Assert.IsNotNull(floor3Panel);
        }

        [TestMethod]
        public void ShouldReturnNullWhenUnknownFloorPanelRequested()
        {
            var bank = new Bank(2, 1..3);

            var floor4Panel = bank.GetFloorElevatorCallPanelFor(4);
            Assert.IsNull(floor4Panel);
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallDownButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var elevators = bank.Elevators;
            Assert.AreEqual(2, elevators.Count());

            var elevator1 = elevators.First();
            var elevator2 = elevators.Last();

            elevator1.Start();
            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(5).isOk);
            ElevatorTest.WaitForElevatorToReachFloor(5, elevator1);

            elevator2.Start();
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(1).isOk);
            ElevatorTest.WaitForElevatorToReachFloor(1, elevator2);

            Assert.AreEqual(5, elevator1.CurrentFloorNumber);
            Assert.AreEqual(1, elevator2.CurrentFloorNumber);

            var thirdFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(3);
            Assert.IsNotNull(thirdFloorElevatorCallPanel);

            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.SetPressedTo(true));
            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.IsPressed);

            elevator1.RequestStopAtFloorNumber(3);
            ElevatorTest.WaitForElevatorToReachFloor(3, elevator1);
            Assert.IsFalse(thirdFloorElevatorCallPanel.DownButton.IsPressed);

            elevator1.Stop();
            elevator2.Stop();
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallUpButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var elevators = bank.Elevators;
            Assert.AreEqual(2, elevators.Count());

            var elevator1 = elevators.First(); //Control Elevators
            var elevator2 = elevators.Last();  //  via Adaptors

            elevator1.Start();
            elevator1.RequestStopAtFloorNumber(5);
            ElevatorTest.WaitForElevatorToReachFloor(5, elevator1);
            Assert.AreEqual(5, elevator1.CurrentFloorNumber);

            elevator2.Start();
            elevator2.RequestStopAtFloorNumber(2);
            ElevatorTest.WaitForElevatorToReachFloor(2, elevator2);
            Assert.AreEqual(2, elevator2.CurrentFloorNumber);

            var firstFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(1);
            Assert.IsNotNull(firstFloorElevatorCallPanel);

            Assert.IsTrue(firstFloorElevatorCallPanel.UpButton.SetPressedTo(true));

            elevator1.RequestStopAtFloorNumber(1);
            ElevatorTest.WaitForElevatorToReachFloor(1, elevator1);
            Assert.IsFalse(firstFloorElevatorCallPanel.UpButton.IsPressed);

            elevator1.Stop();
            elevator2.Stop();
        }

        #endregion
    }
}