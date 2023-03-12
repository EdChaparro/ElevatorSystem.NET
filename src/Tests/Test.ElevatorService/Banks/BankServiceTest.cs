using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorService.Tests.Strategies;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Banks
{
    [TestClass]
    public class BankServiceTest
    {
        public BankServiceTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

        [TestMethod]
        public void ShouldSendElevatorsToLowestFloorWithDoorOpenOnStart()
        {
            var bank = new Bank(2, 1..5);
            var elevators = bank.Elevators.ToList();
            Assert.AreEqual(2, elevators.Count());

            var elevatorRegistry = new ElevatorServiceRegistry();
            var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

            var bankRegistry = new BankServiceRegistry(new ElevatorServiceRegistry());
            var bankRunner = new BankServiceRunner(bankRegistry, elevatorRunner);

            elevatorRegistry.Register(elevators[0], elevators[1]);

            bankRegistry.Register(bank);
            Assert.IsTrue(bankRunner.Start(bank));

            Assert.IsTrue(elevatorRunner.Start(elevators[0]));
            Assert.IsTrue(elevatorRunner.Start(elevators[1]));

            foreach (var elevator in elevators)
            {
                Assert.AreEqual(bank.LowestFloorNbr, elevator.CurrentFloorNumber);
                Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
            }

            Assert.IsTrue(elevatorRunner.StopAsync(elevators[0]).Result);
            Assert.IsTrue(elevatorRunner.StopAsync(elevators[1]).Result);
            Assert.IsTrue(bankRunner.StopAsync(bank).Result);
        }

        [TestMethod]
        public void ShouldTrackAssignedElevators()
        {
            //TODO: Fix Me -- This test will fail when the Bank has more than one elevator (and other elevator services are not running)
            var bank = new Bank(1, 1..50);
            var elevator = bank.Elevators.First(); //First idle elevator will be assigned

            var elevatorRegistry = new ElevatorServiceRegistry();
            var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

            var bankRegistry = new BankServiceRegistry(new ElevatorServiceRegistry());
            var bankRunner = new BankServiceRunner(bankRegistry, elevatorRunner);

            elevatorRegistry.Register(elevator);
            Assert.IsTrue(elevatorRunner.Start(elevator));

            bankRegistry.Register(bank);

            var service = bankRegistry.Get(bank) as BankService;    //TODO: Eliminate casting
            Assert.IsNotNull(service);

            Assert.IsFalse(service.AssignedFloorStops.Any());

            Assert.IsTrue(bankRunner.Start(bank));

            Assert.IsTrue(bank.PressButtonForFloorNumber(14, Direction.Down));

            Thread.Sleep(200);  //Give the Engine a chance to do its thing

            Assert.IsTrue(service.AssignedFloorStops.Any());

            TestStrategy.WaitForElevatorToReachFloor(14, elevator, 15);

            Assert.IsFalse(service.AssignedFloorStops.Any());   //Cache should be clear

            Assert.IsTrue(bankRunner.StopAsync(bank).Result);
        }

        [TestMethod]
        public void ShouldChangeElevatorDirectionWhenIdle()
        {
            var bank = new Bank(2, 1..10);
            var e1 = bank.Elevators.First();
            var e2 = bank.Elevators.Last();

            e1.Name = "Test Elevator 1";
            e2.Name = "Test Elevator 2";

            var elevatorRegistry = new ElevatorServiceRegistry();
            var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

            var bankRegistry = new BankServiceRegistry(new ElevatorServiceRegistry());
            var bankRunner = new BankServiceRunner(bankRegistry, elevatorRunner);

            elevatorRegistry.Register(e1, e2);
            Assert.IsTrue(elevatorRunner.Start(e1));
            Assert.IsTrue(elevatorRunner.Start(e2));

            Assert.AreEqual(Direction.Up, e1.Direction);
            Assert.AreEqual(Direction.Up, e2.Direction);

            bankRegistry.Register(bank);
            Assert.IsTrue(bankRunner.Start(bank));

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Down));

            //First idle elevator will be selected by Bank Engine
            TestStrategy.WaitForElevatorToReachFloor(5, e1);

            Assert.IsTrue(e1.IsIdle);  //Should change direction when elevator idle
            Assert.AreEqual(Direction.Down, e1.Direction);
            Assert.AreEqual(5, e1.CurrentFloorNumber);
            Assert.IsTrue(e1.PressButtonForFloorNumber(1));  //Send it back down

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));
            //Expecting second elevator to respond to Floor Up Call Request
            TestStrategy.WaitForElevatorToReachFloor(5, e2);
            Assert.IsTrue(e2.IsIdle);  //Should change direction when elevator idle
            Assert.AreEqual(Direction.Up, e2.Direction);
            Assert.AreEqual(5, e2.CurrentFloorNumber);

            Assert.IsTrue(elevatorRunner.StopAsync(e1).Result);
            Assert.IsTrue(elevatorRunner.StopAsync(e2).Result);

            Assert.IsTrue(bankRunner.StopAsync(bank).Result);
        }
    }
}
