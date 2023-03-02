using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService;

public class ElevatorServiceRunner
{
    //public static void Main(string[] args)
    //{
    //    var host = CreateHostBuilder(args)
    //        .Build();

    //    host.RunAsync();

    //    Console.WriteLine("Engine started, waiting 5 second before shutdown");
    //    Thread.Sleep(5000);

    //    Console.WriteLine("Shutting down Engine");
    //    host.StopAsync();
    //}

    public static void Main(string[] args)
    {
        var bank = new Bank(2, 1..10);

        var service = GetElevatorEngineFor(bank.Elevators.First());

        var cancellationToken = new CancellationToken();
        service.StartAsync(cancellationToken);

        Console.WriteLine("Engine started directly, waiting 5 second before shutdown");
        Console.WriteLine($"IsRunning: {service.IsRunning}");

        Thread.Sleep(5000);

        Console.WriteLine("Shutting down Engine");
        service.StopAsync(cancellationToken);

        Console.WriteLine($"IsRunning: {service.IsRunning}");
    }

    public static IBackgroundService GetElevatorEngineFor(Elevator elevator)
    {
        return new ElevatorService(elevator);
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var bank = new Bank(2, 1..10);

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<Elevator>(bank.Elevators.First());
                services.AddHostedService<ElevatorService>();
            });
    }
}