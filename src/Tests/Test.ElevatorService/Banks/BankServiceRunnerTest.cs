using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Banks;

[TestClass]
public class BankServiceRunnerTest
{
    public BankServiceRunnerTest()
    {
        Configuration.EngineSleepIntervalInMilliseconds = 100;
    }

    [TestMethod]
    public void ShouldStopRunningServiceWhenUnRegistered()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        var elevatorRunner = new ElevatorServiceRunner(new ElevatorServiceRegistry());

        var runner = new BankServiceRunner(registry, elevatorRunner);

        var bank = new Bank(2, 1..10);
        registry.Register(bank);

        var service = registry.Get(bank);
        Assert.IsNotNull(service);
        Assert.IsFalse(service.IsRunning);

        runner.Start(bank);
        Assert.IsTrue(service.IsRunning);

        registry.UnRegister(bank);
        Assert.AreEqual(0, registry.Count);

        Assert.IsFalse(service.IsRunning);
    }

    [TestMethod]
    public void ShouldNotStartElevatorServiceUponRegistration()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        var elevatorRunner = new ElevatorServiceRunner(new ElevatorServiceRegistry());

        var runner = new BankServiceRunner(registry, elevatorRunner);
        Assert.AreEqual(0, registry.Count);

        var bank = new Bank(2, 1..10);
        registry.Register(bank);

        Assert.AreEqual(1, registry.Count);

        Assert.IsFalse(runner.IsRunning(bank));
    }

    [TestMethod]
    public void ShouldStartElevatorService()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        var elevatorRunner = new ElevatorServiceRunner(new ElevatorServiceRegistry());

        var runner = new BankServiceRunner(registry, elevatorRunner);

        var bank = new Bank(2, 1..10);
        registry.Register(bank);

        Assert.IsFalse(runner.IsRunning(bank));
        runner.Start(bank);
        Assert.IsTrue(runner.IsRunning(bank));
    }

    [TestMethod]
    public void ShouldStopElevatorService()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        var elevatorRunner = new ElevatorServiceRunner(new ElevatorServiceRegistry());

        var runner = new BankServiceRunner(registry, elevatorRunner);

        var bank = new Bank(2, 1..10);
        registry.Register(bank);

        runner.Start(bank);
        Assert.IsTrue(runner.IsRunning(bank));

        var isStopped = runner.StopAsync(bank);

        Assert.IsTrue(isStopped.Result);
        Assert.IsFalse(runner.IsRunning(bank));
    }
}