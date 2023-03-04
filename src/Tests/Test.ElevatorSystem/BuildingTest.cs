using IntrepidProducts.ElevatorSystem.Banks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Tests
{
    [TestClass]
    public class BuildingTest
    {
        #region Banks
        [TestMethod]
        public void ShouldKeepElevatorBankCount()
        {
            var bank1 = new Bank(2, 1, 2);
            var bank2 = new Bank(2, 3, 4);

            var building = new Building(bank1, bank2);
            Assert.AreEqual(2, building.NumberOfBanks);
        }

        [TestMethod]
        public void ShouldGetBankById()
        {
            var bank1 = new Bank(2, 1, 2);
            var bank2 = new Bank(2, 3, 4);

            var building = new Building(bank1, bank2);
            Assert.AreEqual(bank1, building.GetBank(bank1.Id));
            Assert.AreEqual(bank2, building.GetBank(bank2.Id));
        }

        [TestMethod]
        public void ShouldGetBankByName()
        {
            var bank1 = new Bank(2, 1, 2)
            {
                Name = "A"
            };

            var bank2 = new Bank(2, 3, 4)
            {
                Name = "B"
            };

            var building = new Building(bank1, bank2);
            Assert.AreEqual(bank1, building.GetBank("A"));
            Assert.AreEqual(bank2, building.GetBank("B"));
        }

        [TestMethod]
        public void ShouldReturnAllBanks()
        {
            var bank1 = new Bank(2, 1, 2);
            var bank2 = new Bank(2, 3, 4);

            var building = new Building(bank1, bank2);

            CollectionAssert.AreEqual
                (new List<IBank> { bank1, bank2 }, building.Banks.ToList());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueElevatorBanks()
        {
            var bank = new Bank(1, 1, 2);
            var dup = bank;

            new Building(bank, dup);
        }

        [TestMethod]
        public void ShouldSupportAddingBanksAfterInstantiation()
        {
            var building = new Building();
            Assert.AreEqual(0, building.NumberOfBanks);

            building.Add(new Bank(1, 1..3));
            Assert.AreEqual(1, building.NumberOfBanks);
        }

        #endregion

        #region Floors
        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var bank1 = new Bank(3, 1, 2);
            var bank2 = new Bank(3, 1, 3, 4);

            var building = new Building(bank1, bank2);

            Assert.AreEqual(4, building.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportFloorCountBasedOnHighNumber()
        {
            var bank = new Bank(4, 1, 3, 40);

            var building = new Building(bank);

            Assert.AreEqual(40, building.NumberOfFloors);   //Gaps are ignored
        }

        [TestMethod]
        public void ShouldReportLowestFloorNumber()
        {
            var bank1 = new Bank(2, 2, 9);
            var bank2 = new Bank(2, 3, 4, 6);

            var building = new Building(bank1, bank2);

            Assert.AreEqual(2, building.LowestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportHighestFloorNumber()
        {
            var bank1 = new Bank(1, 2, 9);
            var bank2 = new Bank(1, 3, 4, 6);

            var building = new Building(bank1, bank2);

            Assert.AreEqual(9, building.HighestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportOrderedCollectionOfFloorNumbers()
        {
            var bank1 = new Bank(2, 1..4);

            var bank2 = new Bank(2, 4..4);

            var bank3 = new Bank(2, 5..5);

            var building = new Building(bank1, bank2, bank3);

            var expectedFloorList = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            CollectionAssert.AreEqual(expectedFloorList, building.OrderedFloorNumbers.ToArray());
        }

        #endregion
    }
}