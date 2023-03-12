using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Elevators;

[TestClass]
public class ElevatorServiceRunnerTest
{
    public ElevatorServiceRunnerTest()
    {
        Configuration.EngineSleepIntervalInMilliseconds = 100;
    }

    [TestMethod]
    public void ShouldStopRunningServiceWhenUnRegistered()
    {
        var elevatorRegistry = new ElevatorServiceRegistry();
        var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

        var elevator = new Elevator(1..10);
        elevatorRegistry.Register(elevator);

        var service = elevatorRegistry.Get(elevator);
        Assert.IsNotNull(service);
        Assert.IsFalse(service.IsRunning);

        Assert.IsTrue(elevatorRunner.Start(elevator));
        Assert.IsTrue(service.IsRunning);

        elevatorRegistry.UnRegister(elevator);
        Assert.AreEqual(0, elevatorRegistry.Count);

        Assert.IsFalse(service.IsRunning);
    }

    [TestMethod]
    public void ShouldNotStartElevatorServiceUponRegistration()
    {
        var elevatorRegistry = new ElevatorServiceRegistry();
        var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

        Assert.AreEqual(0, elevatorRegistry.Count);

        var elevator = new Elevator(1..10);
        elevatorRegistry.Register(elevator);

        Assert.AreEqual(1, elevatorRegistry.Count);

        Assert.IsFalse(elevatorRunner.IsRunning(elevator));
    }

    [TestMethod]
    public void ShouldStartElevatorService()
    {
        var elevatorRegistry = new ElevatorServiceRegistry();
        var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

        var elevator = new Elevator(1..10);
        elevatorRegistry.Register(elevator);

        Assert.IsFalse(elevatorRunner.IsRunning(elevator));
        Assert.IsTrue(elevatorRunner.Start(elevator));
        Assert.IsTrue(elevatorRunner.IsRunning(elevator));
    }

    [TestMethod]
    public void ShouldNotStartElevatorServiceWhenDisabled()
    {
        var elevatorRegistry = new ElevatorServiceRegistry();
        var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

        var elevator = new Elevator(1..10);
        elevatorRegistry.Register(elevator);

        elevator.IsEnabled = false;

        Assert.IsFalse(elevatorRunner.IsRunning(elevator));
        Assert.IsFalse(elevatorRunner.Start(elevator));
        Assert.IsFalse(elevatorRunner.IsRunning(elevator));
    }

    [TestMethod]
    public void ShouldStopElevatorService()
    {
        var elevatorRegistry = new ElevatorServiceRegistry();
        var elevatorRunner = new ElevatorServiceRunner(elevatorRegistry);

        var elevator = new Elevator(1..10);
        elevatorRegistry.Register(elevator);

        Assert.IsTrue(elevatorRunner.Start(elevator));
        Assert.IsTrue(elevatorRunner.IsRunning(elevator));

        var isStopped = elevatorRunner.StopAsync(elevator);

        Assert.IsTrue(isStopped.Result);
        Assert.IsFalse(elevatorRunner.IsRunning(elevator));
    }
}