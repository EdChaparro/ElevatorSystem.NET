using System.Collections.Generic;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IntrepidProducts.ElevatorSystem.Tests.Banks
{
    [TestClass]
    public class StrategyTest
    {
        [TestMethod]
        public void ShouldPassUnassignedFloorStopsToNextStrategy()
        {
            var bank = new Bank(2, 1..10);

            var mockStrategy = new Mock<IStrategy>();

            var strategy = new TestStrategy(bank, mockStrategy.Object);

            var requestedFloorStops = new List<int> {5, 6, 7};

            //Test Strategy will only assign first floorstop
            strategy.AssignElevators(requestedFloorStops, Direction.Down);

            //Next-Strategy should have been called with unassigned floors
            mockStrategy.Verify(x =>
                x.AssignElevators(new List<int> {6, 7}, Direction.Down), Times.Once);
        }
    }
}