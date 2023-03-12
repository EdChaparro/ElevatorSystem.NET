using IntrepidProducts.ElevatorService.Banks;
using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Banks;

[TestClass]
public class BankServiceRegistryTest
{
    public BankServiceRegistryTest()
    {
        Configuration.EngineSleepIntervalInMilliseconds = 100;
    }

    [TestMethod]
    public void ShouldRegisterBankService()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        Assert.AreEqual(0, registry.Count);

        var bank = new Bank(2, 1..10);
        registry.Register(bank);

        Assert.AreEqual(1, registry.Count);
    }

    [TestMethod]
    public void ShouldRegisterMultipleBankServices()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());
        Assert.AreEqual(0, registry.Count);

        var bank1 = new Bank(2, 1..10);
        var bank2 = new Bank(2, 1..10);
        registry.Register(bank1, bank2);

        Assert.AreEqual(2, registry.Count);
    }

    [TestMethod]
    public void ShouldUnRegisterBankService()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());

        var bank = new Bank(2, 1..10);
        registry.Register(bank);
        Assert.AreEqual(1, registry.Count);

        registry.UnRegister(bank);
        Assert.AreEqual(0, registry.Count);
    }

    [TestMethod]
    public void ShouldUnRegisterMultipleBankervices()
    {
        var registry = new BankServiceRegistry(new ElevatorServiceRegistry());

        var bank1 = new Bank(2, 1..10);
        var bank2 = new Bank(2, 1..10);
        var bank3 = new Bank(2, 1..10);

        registry.Register(bank1, bank2, bank3);
        Assert.AreEqual(3, registry.Count);

        registry.UnRegister(bank1, bank3);
        Assert.AreEqual(1, registry.Count);

        Assert.IsFalse(registry.IsRegistered(bank1));
        Assert.IsTrue(registry.IsRegistered(bank2));
        Assert.IsFalse(registry.IsRegistered(bank3));
    }
}