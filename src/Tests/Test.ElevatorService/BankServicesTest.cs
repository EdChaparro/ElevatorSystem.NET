using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorService.Tests
{
    [TestClass]
    public class BankServicesTest
    {
        [TestMethod]
        public void ShouldRegisterElevatorService()
        {
            var services = new BankServices();
            Assert.AreEqual(0, services.Count);

            var bank = new Bank(2, 1..10);
            services.Register(bank);

            Assert.AreEqual(1, services.Count);
        }

        [TestMethod]
        public void ShouldRegisterMultipleElevatorServices()
        {
            var services = new BankServices();
            Assert.AreEqual(0, services.Count);

            var bank1 = new Bank(2, 1..10);
            var bank2 = new Bank(2, 1..10);
            services.Register(bank1, bank2);

            Assert.AreEqual(2, services.Count);
        }

        [TestMethod]
        public void ShouldUnRegisterElevatorService()
        {
            var services = new BankServices();

            var bank = new Bank(2,1..10);
            services.Register(bank);
            Assert.AreEqual(1, services.Count);

            services.UnRegister(bank);
            Assert.AreEqual(0, services.Count);
        }

        [TestMethod]
        public void ShouldStopRunningServiceWhenUnRegistered()
        {
            var services = new BankServices();

            var bank = new Bank(2, 1..10);
            services.Register(bank);

            var service = services.Get(bank);
            Assert.IsNotNull(service);
            Assert.IsFalse(service.IsRunning);

            services.Start(bank);
            Assert.IsTrue(service.IsRunning);

            services.UnRegister(bank);
            Assert.AreEqual(0, services.Count);

            Assert.IsFalse(service.IsRunning);
        }

        [TestMethod]
        public void ShouldUnRegisterMultipleElevatorServices()
        {
            var services = new BankServices();

            var bank1 = new Bank(2,1..10);
            var bank2 = new Bank(2,1..10);
            var bank3 = new Bank(2,1..10);

            services.Register(bank1, bank2, bank3);
            Assert.AreEqual(3, services.Count);

            services.UnRegister(bank1, bank3);
            Assert.AreEqual(1, services.Count);

            Assert.IsFalse(services.IsRegistered(bank1));
            Assert.IsTrue(services.IsRegistered(bank2));
            Assert.IsFalse(services.IsRegistered(bank3));
        }


        [TestMethod]
        public void ShouldNotStartElevatorServiceUponRegistration()
        {
            var services = new BankServices();
            Assert.AreEqual(0, services.Count);

            var bank = new Bank(2,1..10);
            services.Register(bank);

            Assert.AreEqual(1, services.Count);

            Assert.IsFalse(services.IsRunning(bank));
        }

        [TestMethod]
        public void ShouldStartElevatorService()
        {
            var services = new BankServices();

            var bank = new Bank(2, 1..10);
            services.Register(bank);

            Assert.IsFalse(services.IsRunning(bank));
            services.Start(bank);
            Assert.IsTrue(services.IsRunning(bank));
        }


        [TestMethod]
        public void ShouldStopElevatorService()
        {
            var services = new BankServices();

            var bank = new Bank(2, 1..10);
            services.Register(bank);

            services.Start(bank);
            Assert.IsTrue(services.IsRunning(bank));

            var isStopped = services.StopAsync(bank);

            Assert.IsTrue(isStopped.Result);
            Assert.IsFalse(services.IsRunning(bank));
        }
    }
}