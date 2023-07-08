using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorService.Banks;

public class BankServiceConsoleRunner
{
    public static void Main(string[] args)
    {
        var bank = new Bank(2, 1..10);

        var service = GetEngineFor(bank);

        //var cancellationToken = new CancellationToken();
        service.StartAsync();

        Console.WriteLine("Bank Engine started directly, waiting 5 second before shutdown");
        Console.WriteLine($"IsRunning: {service.IsRunning}");

        Thread.Sleep(5000);

        Console.WriteLine("Shutting down Engine");
        service.StopAsync();

        Console.WriteLine($"IsRunning: {service.IsRunning}");
    }

    public static IBackgroundService GetEngineFor(Bank bank)
    {
        var elevatorServiceRegistry = new ElevatorServiceRegistry();

        foreach (var elevator in bank.EnabledElevators)
        {
            elevatorServiceRegistry.Register(elevator);
        }

        return new BankService(bank, elevatorServiceRegistry);
    }
}