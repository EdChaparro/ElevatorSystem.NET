using System;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests
{
    [TestClass]
    public class BuildingTest
    {
        [TestMethod]
        public void ShouldKeepElevatorBankCount()
        {
            var bank1 = new Bank(2,new Floor(1), new Floor(2));
            var bank2 = new Bank(2,new Floor(3), new Floor(4));

            var building = new Building(bank1, bank2);
            Assert.AreEqual(2, building.NumberOfBanks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueElevatorBanks()
        {
            var bank = new Bank(1, new Floor(1), new Floor(2));
            var dup = bank;

            var building = new Building(bank, dup);
        }

        #region Floors
        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var bank1 = new Bank(3,new Floor(1), new Floor(2));
            var bank2 = new Bank(3,new Floor(1), new Floor(3), new Floor(4));

            var building = new Building(bank1, bank2);

            Assert.AreEqual(4, building.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportFloorCountBasedOnHighNumber()
        {
            var bank = new Bank(4, new Floor(1), new Floor(3), new Floor(40));

            var building = new Building(bank);

            Assert.AreEqual(40, building.NumberOfFloors);   //Gaps are ignored
        }

        [TestMethod]
        public void ShouldReportLowestFloorNumber()
        {
            var bank1 = new Bank(2,new Floor(2), new Floor(9));
            var bank2 = new Bank(2, new Floor(3), new Floor(4), new Floor(6));

            var building = new Building(bank1, bank2);

            Assert.AreEqual(2, building.LowestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportHighestFloorNumber()
        {
            var bank1 = new Bank(1,new Floor(2), new Floor(9));
            var bank2 = new Bank(1,new Floor(3), new Floor(4), new Floor(6));

            var building = new Building(bank1, bank2);

            Assert.AreEqual(9, building.HighestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportOrderedCollectionOfFloorNumbers()
        {
            var bank1 = new Bank(2,
                new Floor(1),
                new Floor(2),
                new Floor(3),
                new Floor(4)
            );

            var bank2 = new Bank(2,
                new Floor(4),
                new Floor(5),
                new Floor(6),
                new Floor(7)
            );

            var bank3 = new Bank(2,
                new Floor(5),
                new Floor(6),
                new Floor(7),
                new Floor(8),
                new Floor(9)
            );

            var building = new Building(bank1, bank2, bank3);

            var expectedFloorList = new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Assert.IsTrue(expectedFloorList.SequenceEqual(building.OrderedFloorNumbers));
        }

        #endregion
    }
}