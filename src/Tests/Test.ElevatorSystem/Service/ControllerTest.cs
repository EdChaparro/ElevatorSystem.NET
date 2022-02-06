using System;
using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Service
{
    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueElevatorBanks()
        {
            var bank = new Bank(1, new Floor(1), new Floor(2));
            var dup = bank;

            new Controller(bank, dup);
        }
    }
}