using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService;

public class ElevatorEngine : AbstractBackgroundService
{
    public ElevatorEngine(Elevator elevator)
    {
        SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;

        Elevator = elevator;
    }

    protected int SleepIntervalInMilliseconds { get; set; }

    private Elevator Elevator { get; }

    private void NavigateToNextFloorStop()
    {
        if (Elevator.DoorStatus == DoorStatus.Open)
        {
            Elevator.DoorStatus = DoorStatus.Closed;
        }

        var currentFloorNumber = Elevator.CurrentFloorNumber;

        switch (Elevator.Direction)
        {
            case Direction.Down:
                if (currentFloorNumber != Elevator.OrderedFloorNumbers.Min())
                {
                    currentFloorNumber--;
                }
                break;
            case Direction.Up:
                if (currentFloorNumber != Elevator.OrderedFloorNumbers.Max())
                {
                    currentFloorNumber++;
                }
                break;
        }

        if (currentFloorNumber == Elevator.CurrentFloorNumber)
        {
            return; //Reached termination point
        }

        SetDirectionToStopAt(currentFloorNumber);
        Elevator.CurrentFloorNumber = currentFloorNumber;
    }

    private void SetDirectionToStopAt(int floorNbr)
    {
        Elevator.Direction = (floorNbr < Elevator.CurrentFloorNumber) ? Direction.Down : Direction.Up;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Real ElevatorEngine started at: {DateTimeOffset.Now}, Id: {Elevator.Id}");
        Console.WriteLine($"SleepIntervalInMilliseconds {SleepIntervalInMilliseconds}");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Elevator.RequestedFloorStops.Any())
            {
                NavigateToNextFloorStop();
            }

            Console.WriteLine($"Real ElevatorEngine running at: {DateTimeOffset.Now}");

            await Task.Delay(SleepIntervalInMilliseconds, stoppingToken);
        }
    }
}