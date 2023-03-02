using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests
{
    [TestClass]
    public class ElevatorServicesTest
    {
        [TestMethod]
        public void ShouldRegisterElevatorService()
        {
            var service = new ElevatorServices();
            Assert.AreEqual(0, service.Count);

            var elevator = new Elevator(1..10);
            service.Register(elevator);

            Assert.AreEqual(1, service.Count);
        }

        [TestMethod]
        public void ShouldRegisterMultipleElevatorServices()
        {
            var service = new ElevatorServices();
            Assert.AreEqual(0, service.Count);

            var elevator1 = new Elevator(1..10);
            var elevator2 = new Elevator(1..10);
            service.Register(elevator1, elevator2);

            Assert.AreEqual(2, service.Count);
        }

        [TestMethod]
        public void ShouldUnRegisterElevatorService()
        {
            var service = new ElevatorServices();

            var elevator = new Elevator(1..10);
            service.Register(elevator);
            Assert.AreEqual(1, service.Count);

            service.UnRegister(elevator);
            Assert.AreEqual(0, service.Count);
        }

        [TestMethod]
        public void ShouldUnRegisterMultipleElevatorServices()
        {
            var service = new ElevatorServices();

            var elevator1 = new Elevator(1..10);
            var elevator2 = new Elevator(1..10);
            var elevator3 = new Elevator(1..10);

            service.Register(elevator1, elevator2, elevator3);
            Assert.AreEqual(3, service.Count);

            service.UnRegister(elevator1, elevator3);
            Assert.AreEqual(1, service.Count);

            Assert.IsFalse(service.IsRegistered(elevator1));
            Assert.IsTrue(service.IsRegistered(elevator2));
            Assert.IsFalse(service.IsRegistered(elevator3));
        }


        [TestMethod]
        public void ShouldNotStartElevatorServiceUponRegistration()
        {
            var service = new ElevatorServices();
            Assert.AreEqual(0, service.Count);

            var elevator = new Elevator(1..10);
            service.Register(elevator);

            Assert.AreEqual(1, service.Count);

            Assert.IsFalse(service.IsRunning(elevator));
        }

        [TestMethod]
        public void ShouldStartElevatorService()
        {
            var service = new ElevatorServices();

            var elevator = new Elevator(1..10);
            service.Register(elevator);

            Assert.IsFalse(service.IsRunning(elevator));
            service.Start(elevator);
            Assert.IsTrue(service.IsRunning(elevator));
        }


        [TestMethod]
        public void ShouldStopElevatorService()
        {
            var service = new ElevatorServices();

            var elevator = new Elevator(1..10);
            service.Register(elevator);

            service.Start(elevator);
            Assert.IsTrue(service.IsRunning(elevator));

            var isStopped = service.StopAsync(elevator);

            Assert.IsTrue(isStopped.Result);
            Assert.IsFalse(service.IsRunning(elevator));
        }
    }
}