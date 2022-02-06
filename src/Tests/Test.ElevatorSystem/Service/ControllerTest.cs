using System;
using System.Linq;
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
            var bank = new Bank(1,
                new Floor(1),
                new Floor(2));
            var dup = bank;

            new Controller(bank, dup);
        }

        [TestMethod]
        public void ShouldSendElevatorsToLowestFloorWithDoorOpenOnStart()
        {
            var bank = new Bank(2,
                new Floor(1),
                new Floor(2),
                new Floor(3),
                new Floor(4),
                new Floor(5));

            bank.Start();
            var states = bank.ElevatorStates;
            Assert.AreEqual(2, states.Count());

            foreach (var state in states)
            {
                Assert.AreEqual(bank.LowestFloorNbr, state.CurrentFloorNumber);
                Assert.AreEqual(DoorStatus.Open, state.DoorStatus);
            }

            bank.Stop();
        }
    }
}