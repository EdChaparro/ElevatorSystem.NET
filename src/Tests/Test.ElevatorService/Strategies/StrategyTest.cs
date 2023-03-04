using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;
using Moq;

namespace IntrepidProducts.ElevatorService.Tests.Strategies
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

            var requestedFloorStops = new List<int> { 5, 6, 7 };

            //Test Strategy will only assign first floorstop
            strategy.AssignElevators(requestedFloorStops, Direction.Down);

            //Next-Strategy should have been called with unassigned floors
            mockStrategy.Verify(x =>
                x.AssignElevators(new List<int> { 6, 7 }, Direction.Down), Times.Once);
        }

        [TestMethod]
        public void ShouldCombineAssignmentResultsFromAllStrategies()
        {
            var bank = new Bank(2, 1..10);

            var mockStrategy = new Mock<IStrategy>();

            mockStrategy
                .Setup(x => x.AssignElevators
                    (It.IsAny<List<int>>(), Direction.Down))
                .Returns
                    (new List<RequestedFloorStop> { new RequestedFloorStop(7, Direction.Down) });

            var strategy = new TestStrategy(bank, mockStrategy.Object);

            var requestedFloorStops = new List<int> { 5, 6, 7 };

            //Test Strategy will assign first floor stop
            //Mock Strategy will assign last floor stop
            var assignedFloorStops
                = strategy.AssignElevators(requestedFloorStops, Direction.Down).ToList();

            Assert.AreEqual(2, assignedFloorStops.Count());

            var expectedFloorList = new[] { 5, 7 };
            Assert.IsTrue(expectedFloorList
                .SequenceEqual(assignedFloorStops.Select(x => x.FloorNbr)));
        }
    }
}