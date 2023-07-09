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

            var bankRegistry = new BankServiceRegistry(elevatorRegistry);

            bankRegistry.Register(bank);
            var service = bankRegistry.Get(bank);
            Assert.IsNotNull(service);
            service.StartAsync();
            Assert.IsTrue(service.IsRunning);

            foreach (var elevator in elevators)
            {
                Assert.AreEqual(bank.LowestFloorNbr, elevator.CurrentFloorNumber);
                Assert.AreEqual(DoorStatus.Open, elevator.DoorStatus);
            }

            service.StopAsync();
            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldTrackAssignedElevators()
        {
            var bank = new Bank(2, 1..50);
            var elevator = bank.Elevators.First(); //First idle elevator will be assigned

            var elevatorRegistry = new ElevatorServiceRegistry();

            var bankRegistry = new BankServiceRegistry(elevatorRegistry);
            bankRegistry.Register(bank);

            var service = bankRegistry.Get(bank) as BankService;    //TODO: Eliminate casting
            Assert.IsNotNull(service);

            Assert.IsFalse(service.AssignedFloorStops.Any());

            service.StartAsync();
            Assert.IsTrue(service.IsRunning);

            Assert.IsTrue(bank.PressButtonForFloorNumber(14, Direction.Down));

            Thread.Sleep(200);  //Give the Engine a chance to do its thing

            Assert.IsTrue(service.AssignedFloorStops.Any());

            TestStrategy.WaitForElevatorToReachFloor(14, elevator, 15);

            Assert.IsFalse(service.AssignedFloorStops.Any());   //Cache should be clear

            service.StopAsync();
            Assert.IsFalse(service.IsRunning);
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

            var bankRegistry = new BankServiceRegistry(elevatorRegistry);
            bankRegistry.Register(bank);

            var service = bankRegistry.Get(bank) as BankService;    //TODO: Eliminate casting
            Assert.IsNotNull(service);

            service.StartAsync();
            Assert.IsTrue(service.IsRunning);

            Assert.AreEqual(Direction.Up, e1.Direction);
            Assert.AreEqual(Direction.Up, e2.Direction);

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

            service.StopAsync();
            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldStopRunningServiceWhenUnRegistered()
        {
            var registry = new BankServiceRegistry(new ElevatorServiceRegistry());

            var bank = new Bank(2, 1..10);
            registry.Register(bank);

            var service = registry.Get(bank);
            Assert.IsNotNull(service);
            Assert.IsFalse(service.IsRunning);

            service.StartAsync();
            Assert.IsTrue(service.IsRunning);

            registry.UnRegister(bank);
            Assert.AreEqual(0, registry.Count);

            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldNotStartBankServiceUponRegistration()
        {
            var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
            Assert.AreEqual(0, registry.Count);

            var bank = new Bank(2, 1..10);
            registry.Register(bank);

            Assert.AreEqual(1, registry.Count);

            var service = registry.Get(bank);
            Assert.IsNotNull(service);
            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldStartBankService()
        {
            var elevatorRegistry = new ElevatorServiceRegistry();
            var registry = new BankServiceRegistry(elevatorRegistry);

            var bank = new Bank(2, 1..10);
            registry.Register(bank);

            var service = registry.Get(bank);
            Assert.IsNotNull(service);
            Assert.IsFalse(service.IsRunning);

            service.StartAsync();
            Assert.IsTrue(service.IsRunning);

            service.StopAsync();
        }

        [TestMethod]
        public void ShouldStartElevatorServicesWhenBankStarted()
        {
            var elevatorRegistry = new ElevatorServiceRegistry();
            var bankRegistry = new BankServiceRegistry(elevatorRegistry);

            var bank = new Bank(2, 1..10);
            bankRegistry.Register(bank);

            var bankService = bankRegistry.Get(bank);
            Assert.IsNotNull(bankService);
            Assert.IsFalse(bankService.IsRunning);

            foreach (var elevator in bank.Elevators)
            {
                var elevatorService = elevatorRegistry.Get(elevator);
                Assert.IsNotNull(elevatorService);
                Assert.IsFalse(elevatorService.IsRunning);
            }

            bankService.StartAsync();
            Assert.IsTrue(bankService.IsRunning);

            foreach (var elevator in bank.Elevators)
            {
                var elevatorService = elevatorRegistry.Get(elevator);
                Assert.IsNotNull(elevatorService);
                Assert.IsTrue(elevatorService.IsRunning);
            }

            bankService.StopAsync();
        }

        [TestMethod]
        public void ShouldOnlyStartElevatorServicesWhenElevatorIsEnabled()
        {
            var elevatorRegistry = new ElevatorServiceRegistry();
            var bankRegistry = new BankServiceRegistry(elevatorRegistry);

            var bank = new Bank(2, 1..10);
            bankRegistry.Register(bank);

            var bankService = bankRegistry.Get(bank);
            Assert.IsNotNull(bankService);
            Assert.IsFalse(bankService.IsRunning);

            bank.Elevators.First().IsEnabled = false;

            bankService.StartAsync();

            var elevatorService = elevatorRegistry.Get(bank.Elevators.First());
            Assert.IsNotNull(elevatorService); //Never registered because the Elevator is Disabled
            Assert.IsFalse(elevatorService.IsRunning);

            elevatorService = elevatorRegistry.Get(bank.Elevators.Last());
            Assert.IsNotNull(elevatorService);
            Assert.IsTrue(elevatorService.IsRunning);

            bankService.StopAsync();
        }

        [TestMethod]
        public void ShouldStopBankService()
        {
            var elevatorRegistry = new ElevatorServiceRegistry();
            var bankRegistry = new BankServiceRegistry(elevatorRegistry);

            var bank = new Bank(2, 1..10);
            bankRegistry.Register(bank);

            var bankService = bankRegistry.Get(bank);
            Assert.IsNotNull(bankService);
            Assert.IsFalse(bankService.IsRunning);

            bankService.StartAsync();
            Assert.IsTrue(bankService.IsRunning);

            bankService.StopAsync();
            Assert.IsFalse(bankService.IsRunning);
        }

        [TestMethod]
        public void ShouldStopElevatorServicesWhenBankStopped()
        {
            var elevatorRegistry = new ElevatorServiceRegistry();
            var bankRegistry = new BankServiceRegistry(elevatorRegistry);

            var bank = new Bank(2, 1..10);
            bankRegistry.Register(bank);

            var bankService = bankRegistry.Get(bank);
            Assert.IsNotNull(bankService);

            bankService.StartAsync();
            Assert.IsTrue(bankService.IsRunning);

            foreach (var elevator in bank.Elevators)
            {
                var elevatorService = elevatorRegistry.Get(elevator);
                Assert.IsNotNull(elevatorService);
                Assert.IsTrue(elevatorService.IsRunning);
            }

            bankService.StopAsync();
            Assert.IsFalse(bankService.IsRunning);

            foreach (var elevator in bank.Elevators)
            {
                var elevatorService = elevatorRegistry.Get(elevator);
                Assert.IsNotNull(elevatorService);
                Assert.IsFalse(elevatorService.IsRunning);
            }
        }
    }
}