using System;
using System.Linq;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    /// <summary>
    /// Service loop intended to run in an independent thread.
    /// </summary>
    public class ElevatorEngine : IEngine
    {
        public ElevatorEngine(Elevator elevator)
        {
            Elevator = elevator;

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

        private Elevator Elevator { get; set; }

        public bool IsRunning { get; private set; }
        protected bool IsStopRequested { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; private set; }
        protected CancellationToken CancellationToken { get; private set; }

        protected int SleepIntervalInMilliseconds { get; set; } = 1000;

        public void Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;

            IsRunning = true;

            while (IsStopRequested == false)
            {
                DoEngineLoop();
                bool isCancelled = CancellationToken.WaitHandle.WaitOne(SleepIntervalInMilliseconds);

                if (isCancelled)
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Elevator Engine is not running");
            }

            CancellationTokenSource.Cancel();
            IsStopRequested = true;
            IsRunning = false;
        }

        protected virtual void DoEngineLoop()
        {
            if (Elevator.RequestedFloorStops.Any())
            {
                NavigateToNextFloorStop();
            }

            Thread.Sleep(SleepIntervalInMilliseconds);
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

            //NavigateToNextFloorStop();  //Recursive call
        }


        private void SetDirectionToStopAt(int floorNbr)
        {
            Elevator.Direction = (floorNbr < Elevator.CurrentFloorNumber) ? Direction.Down : Direction.Up;
        }
    }
}