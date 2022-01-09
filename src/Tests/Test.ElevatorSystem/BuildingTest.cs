using System;
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
            var bank1 = new Bank(new Floor(1), new Floor(2));
            var bank2 = new Bank(new Floor(3), new Floor(4));

            var building = new Building(bank1, bank2);
            Assert.AreEqual(2, building.NumberOfBanks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueElevatorBanks()
        {
            var bank = new Bank(new Floor(1), new Floor(2));
            var dup = bank;

            var bankWithFloors = new Building(bank, dup);
        }
    }
}