using System.Linq;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    /// <summary>
    /// Service loop intended to run in an independent thread.
    /// </summary>
    public class ElevatorEngine : AbstractEngine
    {
        public ElevatorEngine(Elevator elevator)
        {
            SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;

            Elevator = elevator;
        }

        private Elevator Elevator { get; }

        protected override void DoEngineLoop()
        {
            if (Elevator.RequestedFloorStops.Any())
            {
                NavigateToNextFloorStop();
            }
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
            Elevator.Direction = (floorNbr < Elevator.CurrentFloorNumber) ? Direction.Down : Direction.Up;
        }
    }
}