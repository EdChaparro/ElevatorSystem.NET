using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Elevators;

public class ElevatorServiceConsoleRunner
{
    public static void Main(string[] args)
    {
        var bank = new Bank(2, 1..10);

        var service = GetEngineFor(bank.Elevators.First());

        var cancellationToken = new CancellationToken();
        service.StartAsync(cancellationToken);

        Console.WriteLine("Elevator Engine started directly, waiting 5 second before shutdown");
        Console.WriteLine($"IsRunning: {service.IsRunning}");

        Thread.Sleep(5000);

        Console.WriteLine("Shutting down Engine");
        service.StopAsync(cancellationToken);

        Console.WriteLine($"IsRunning: {service.IsRunning}");
    }

    public static IBackgroundService GetEngineFor(Elevator elevator)
    {
        return new ElevatorService(elevator);
    }
}