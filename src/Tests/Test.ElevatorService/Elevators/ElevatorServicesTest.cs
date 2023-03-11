using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Elevators
{
    [TestClass]
    public class ElevatorServicesTest
    {
        public ElevatorServicesTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

        [TestMethod]
        public void ShouldRegisterElevatorService()
        {
            var services = new ElevatorServices();
            Assert.AreEqual(0, services.Count);

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            Assert.AreEqual(1, services.Count);
        }

        [TestMethod]
        public void ShouldRegisterMultipleElevatorServices()
        {
            var services = new ElevatorServices();
            Assert.AreEqual(0, services.Count);

            var elevator1 = new Elevator(1..10);
            var elevator2 = new Elevator(1..10);
            services.Register(elevator1, elevator2);

            Assert.AreEqual(2, services.Count);
        }

        [TestMethod]
        public void ShouldUnRegisterElevatorService()
        {
            var services = new ElevatorServices();

            var elevator = new Elevator(1..10);
            services.Register(elevator);
            Assert.AreEqual(1, services.Count);

            services.UnRegister(elevator);
            Assert.AreEqual(0, services.Count);
        }

        [TestMethod]
        public void ShouldStopRunningServiceWhenUnRegistered()
        {
            var services = new ElevatorServices();

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            var service = services.Get(elevator);
            Assert.IsNotNull(service);
            Assert.IsFalse(service.IsRunning);

            Assert.IsTrue(services.Start(elevator));
            Assert.IsTrue(service.IsRunning);

            services.UnRegister(elevator);
            Assert.AreEqual(0, services.Count);

            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldUnRegisterMultipleElevatorServices()
        {
            var services = new ElevatorServices();

            var elevator1 = new Elevator(1..10);
            var elevator2 = new Elevator(1..10);
            var elevator3 = new Elevator(1..10);

            services.Register(elevator1, elevator2, elevator3);
            Assert.AreEqual(3, services.Count);

            services.UnRegister(elevator1, elevator3);
            Assert.AreEqual(1, services.Count);

            Assert.IsFalse(services.IsRegistered(elevator1));
            Assert.IsTrue(services.IsRegistered(elevator2));
            Assert.IsFalse(services.IsRegistered(elevator3));
        }


        [TestMethod]
        public void ShouldNotStartElevatorServiceUponRegistration()
        {
            var services = new ElevatorServices();
            Assert.AreEqual(0, services.Count);

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            Assert.AreEqual(1, services.Count);

            Assert.IsFalse(services.IsRunning(elevator));
        }

        [TestMethod]
        public void ShouldStartElevatorService()
        {
            var services = new ElevatorServices();

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            Assert.IsFalse(services.IsRunning(elevator));
            Assert.IsTrue(services.Start(elevator));
            Assert.IsTrue(services.IsRunning(elevator));
        }

        [TestMethod]
        public void ShouldNotStartElevatorServiceWhenDisabled()
        {
            var services = new ElevatorServices();

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            elevator.IsEnabled = false;

            Assert.IsFalse(services.IsRunning(elevator));
            Assert.IsFalse(services.Start(elevator));
            Assert.IsFalse(services.IsRunning(elevator));
        }


        [TestMethod]
        public void ShouldStopElevatorService()
        {
            var services = new ElevatorServices();

            var elevator = new Elevator(1..10);
            services.Register(elevator);

            Assert.IsTrue(services.Start(elevator));
            Assert.IsTrue(services.IsRunning(elevator));

            var isStopped = services.StopAsync(elevator);

            Assert.IsTrue(isStopped.Result);
            Assert.IsFalse(services.IsRunning(elevator));
        }
    }
}