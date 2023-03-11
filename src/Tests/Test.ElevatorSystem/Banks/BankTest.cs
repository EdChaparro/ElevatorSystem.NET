using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
{
    [TestClass]
    public class BankTest
    {
        public BankTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

        #region Elevators

        [TestMethod]
        public void ShouldKeepElevatorCount()
        {
            var bank = new Bank(2, 1..2);

            Assert.AreEqual(2, bank.NumberOfElevators);
        }

        [TestMethod]
        public void ShouldKeepEnabledElevatorCount()
        {
            var bank = new Bank(2, 1..2);

            bank.Elevators.First().IsEnabled = false;

            Assert.AreEqual(1, bank.EnabledElevators.Count());
            Assert.AreEqual(bank.Elevators.Last(), bank.EnabledElevators.First());
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

        #endregion
    }
}