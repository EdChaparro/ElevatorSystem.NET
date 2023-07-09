using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Elevators
{
    //TODO: Consider creating ONE Service to manage multiple Elevators
    public class ElevatorService : AbstractBackgroundService
    {
        public ElevatorService(Elevator elevator) : base(Configuration.EngineSleepIntervalInMilliseconds)
        {
            Elevator = elevator;
        }

        private Elevator Elevator { get; }

        protected override bool IsOkToStart()
        {
            return Elevator.IsEnabled;
        }

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
            Elevator.Direction = floorNbr < Elevator.CurrentFloorNumber ? Direction.Down : Direction.Up;
        }

        protected override void ServiceLoop()
        {
            if (Elevator.RequestedFloorStops.Any())
            {
                NavigateToNextFloorStop();
            }
        }
    }
}