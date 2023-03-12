using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Elevators;

[TestClass]
public class ElevatorServiceRegistryTest
{
    public ElevatorServiceRegistryTest()
    {
        Configuration.EngineSleepIntervalInMilliseconds = 100;
    }

    [TestMethod]
    public void ShouldRegisterElevatorService()
    {
        var registry = new ElevatorServiceRegistry();
        Assert.AreEqual(0, registry.Count);

        var elevator = new Elevator(1..10);
        registry.Register(elevator);

        Assert.AreEqual(1, registry.Count);
    }

    [TestMethod]
    public void ShouldRegisterMultipleElevatorServices()
    {
        var registry = new ElevatorServiceRegistry();
        Assert.AreEqual(0, registry.Count);

        var elevator1 = new Elevator(1..10);
        var elevator2 = new Elevator(1..10);
        registry.Register(elevator1, elevator2);

        Assert.AreEqual(2, registry.Count);
    }

    [TestMethod]
    public void ShouldUnRegisterElevatorService()
    {
        var registry = new ElevatorServiceRegistry();

        var elevator = new Elevator(1..10);
        registry.Register(elevator);
        Assert.AreEqual(1, registry.Count);

        registry.UnRegister(elevator);
        Assert.AreEqual(0, registry.Count);
    }

    [TestMethod]
    public void ShouldUnRegisterMultipleElevatorServices()
    {
        var registry = new ElevatorServiceRegistry();

        var elevator1 = new Elevator(1..10);
        var elevator2 = new Elevator(1..10);
        var elevator3 = new Elevator(1..10);

        registry.Register(elevator1, elevator2, elevator3);
        Assert.AreEqual(3, registry.Count);

        registry.UnRegister(elevator1, elevator3);
        Assert.AreEqual(1, registry.Count);

        Assert.IsFalse(registry.IsRegistered(elevator1));
        Assert.IsTrue(registry.IsRegistered(elevator2));
        Assert.IsFalse(registry.IsRegistered(elevator3));
    }
}