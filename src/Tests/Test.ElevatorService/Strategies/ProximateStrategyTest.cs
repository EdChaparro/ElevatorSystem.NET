using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Strategies
{
    [TestClass]
    public class ProximateStrategyTest
    {
        [TestMethod]
        public void ShouldAssignProximateElevatorToServiceStopRequests()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            var elevatorRegistry = new ElevatorServiceRegistry();
            var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            elevatorRegistry.Register(elevator1, elevator2);

            Assert.IsTrue(elevatorRunner.Start(elevator1));
            Assert.IsTrue(elevatorRunner.Start(elevator2));

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(8).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(bank.PressButtonForFloorNumber(7, Direction.Up));

            var strategy = new ProximateStrategy(bank);

            var downAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();
            Assert.AreEqual(0, downAssignments.Count);

            var upAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Up).ToList();
            Assert.AreEqual(1, upAssignments.Count());
            var sRFS = upAssignments.First();
            var eRFS = elevator1.RequestedFloorStops
                .Select(x => x.Id == sRFS.Id).FirstOrDefault();
            Assert.IsNotNull(eRFS);

            TestStrategy.WaitForElevatorToReachFloor(5, elevator2);
            Assert.IsTrue(elevator2.PressButtonForFloorNumber(10));
            TestStrategy.WaitForElevatorToReachFloor(10, elevator2);

            Assert.IsTrue(elevator2.PressButtonForFloorNumber(1));
            bank.PressButtonForFloorNumber(4, Direction.Down);
            downAssignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();
            Assert.AreEqual(1, downAssignments.Count);
            sRFS = downAssignments.First();
            eRFS = elevator2.RequestedFloorStops
                .Select(x => x.Id == sRFS.Id).FirstOrDefault();
            Assert.IsNotNull(eRFS);

            Assert.IsTrue(elevatorRunner.StopAsync(elevator1).Result);
            Assert.IsTrue(elevatorRunner.StopAsync(elevator2).Result);
        }

        [TestMethod]
        public void ShouldNotAssignWhenNoProximateElevatorsAreAvailable()
        {
            var bank = new Bank(2, 1..10);
            var elevator1 = bank.Elevators.First();
            var elevator2 = bank.Elevators.Last();

            elevator1.Name = "Test Elevator 1";
            elevator2.Name = "Test Elevator 2";

            var strategy = new ProximateStrategy(bank);

            Assert.IsTrue(elevator1.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(elevator2.RequestStopAtFloorNumber(6).isOk);
            Assert.IsTrue(bank.PressButtonForFloorNumber(2, Direction.Down));

            var assignments = strategy.AssignElevators
            (bank.GetRequestedFloorStops().Select(x => x.FloorNbr).ToList(),
                Direction.Down).ToList();

            Assert.AreEqual(0, assignments.Count);
        }

        [TestMethod]
        public void ShouldNotAssignElevatorWithDirectionalConflicts()
        {
            var bank = new Bank(1, 1..10);
            var elevator = bank.Elevators.First();

            var elevatorRegistry = new ElevatorServiceRegistry();
            var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

            elevatorRegistry.Register(elevator);
            Assert.IsTrue(elevatorRunner.Start(elevator));

            Assert.IsTrue(bank.PressButtonForFloorNumber(6, Direction.Down));
            Assert.IsTrue(bank.PressButtonForFloorNumber(9, Direction.Up));

            var requestedFloorStops =
                bank.GetRequestedFloorStops(Direction.Down)
                    .Select(x => x.FloorNbr).ToList();

            var idleStrategy = new IdleStrategy(bank);
            var proximateStrategy = new ProximateStrategy(bank);

            var idleAssignments = idleStrategy.AssignElevators
                (requestedFloorStops, Direction.Down);

            Assert.AreEqual(1, idleAssignments.Count());
            var idleAssignment = idleAssignments.First();
            Assert.AreEqual(6, idleAssignment.FloorNbr);

            var proximateAssignments = proximateStrategy.AssignElevators
                (requestedFloorStops, Direction.Up);
            Assert.AreEqual(0, proximateAssignments.Count());

            TestStrategy.WaitForElevatorToReachFloor(6, elevator);
            elevator.RequestStopAtFloorNumber(8);

            proximateAssignments = proximateStrategy.AssignElevators
                (bank.GetRequestedFloorStops(Direction.Up)
                    .Select(x => x.FloorNbr).ToList(), Direction.Up);
            Assert.AreEqual(1, proximateAssignments.Count());

            Assert.IsTrue(elevatorRunner.StopAsync(elevator).Result);
        }
    }
}