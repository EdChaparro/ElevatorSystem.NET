using System;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class BankTest
    {
        #region Elevators

        [TestMethod]
        public void ShouldKeepElevatorCount()
        {
            var bank = new Bank(new Floor(1), new Floor(2));
            Assert.AreEqual(0, bank.NumberOfElevators);

            bank.Add(new Elevator(), new Elevator());
            Assert.AreEqual(2, bank.NumberOfElevators);
        }

        [TestMethod]
        public void ShouldOnlyAcceptUniqueElevators()
        {
            var e1 = new Elevator();
            var dup = e1;

            var e2 = new Elevator();
            var e3 = new Elevator();

            var elevatorBank = new Bank(new Floor(1), new Floor(2));

            Assert.IsTrue(elevatorBank.Add(e1));
            Assert.IsFalse(elevatorBank.Add(dup));

            Assert.IsTrue(elevatorBank.Add(e2));
            Assert.IsTrue(elevatorBank.Add(e3));

            Assert.AreEqual(3, elevatorBank.NumberOfElevators);
        }

        [TestMethod]
        public void ShouldOnlyAcceptCollectionOfElevatorsWhenValid()
        {
            var e1 = new Elevator();
            var dup1 = e1;

            var e2 = new Elevator();
            var e3 = new Elevator();

            var elevatorBank = new Bank(new Floor(1), new Floor(2));

            Assert.IsFalse(elevatorBank.Add(e1, dup1, e2, e3));
            Assert.AreEqual(0, elevatorBank.NumberOfElevators); //Entire collection rejected

            Assert.IsTrue(elevatorBank.Add(e1, e2, e3));
            Assert.AreEqual(3, elevatorBank.NumberOfElevators);
        }

        #endregion

        #region Floors

        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var bankWithFloors = new Bank(new Floor(1), new Floor(2));
            Assert.AreEqual(2, bankWithFloors.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportLowestFloorNumber()
        {
            var bankWithFloors = new Bank(new Floor(1), new Floor(7), new Floor(5));
            Assert.AreEqual(1, bankWithFloors.LowestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportHighestFloorNumber()
        {
            var bank = new Bank(new Floor(1), new Floor(7), new Floor(5));
            Assert.AreEqual(7, bank.HighestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportOrderedCollectionOfFloorNumbers()
        {
            var bank = new Bank(new Floor(1), new Floor(7), new Floor(5));

            var expectedFloorList = new [] { 1, 5, 7 };
            Assert.IsTrue(expectedFloorList.SequenceEqual(bank.OrderedFloorNumbers));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueFloors()
        {
            var bank = new Bank(
                new Floor(1), new Floor(2), new Floor(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAcceptBanksWithLessThanTwoFloors()
        {
            var bank = new Bank(new Floor(1));
        }

        [TestMethod]
        public void ShouldOnlyIncludeAppropriateCallButtonForEachFloor()
        {
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            Assert.IsNull(floor1.Panel);
            Assert.IsNull(floor2.Panel);
            Assert.IsNull(floor3.Panel);

            var bank = new Bank(floor1, floor2, floor3);

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
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            Assert.IsNull(floor1.Panel);
            Assert.IsNull(floor2.Panel);
            Assert.IsNull(floor3.Panel);

            var bank = new Bank(floor2, floor3, floor1); //Order doesn't matter

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

        #endregion
    }
}