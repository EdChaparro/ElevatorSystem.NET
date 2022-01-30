using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class FauxElevatorCommandAdapter : IElevatorCommandAdapter
    {
        public FauxElevatorCommandAdapter(Bank bank, Elevator elevator,
            int moveLatencyInMilliseconds = 1000)
        {
            _bank = bank;
            _elevator = elevator;
            SetElevatorObservability();
        }

        private readonly Bank _bank;
        private readonly Elevator _elevator;

        public Guid ElevatorId => _elevator.Id;

        private void SetDirectionToStopAt(int floorNbr)
        {
            if (floorNbr == _bank.LowestFloorNbr)
            {
                _elevator.Direction = Direction.Up;
                return;
            }

            if (floorNbr == _bank.HighestFloorNbr)
            {
                _elevator.Direction = Direction.Down;
                return;
            }

            _elevator.Direction = _elevator.FloorNumber < floorNbr
                ? Direction.Up
                : Direction.Down;
        }

        public bool StopAt(int floorNbr)
        {
            _elevator.DoorStatus = DoorStatus.Closed;

            SetDirectionToStopAt(floorNbr);

            //TODO: Modify to simulate elevator movement latency
            if (_elevator.SetFloorNumberTo(floorNbr))
            {
                _elevator.DoorStatus = DoorStatus.Open;
                return true;
            }

            return false;
        }

        public ElevatorStatus Status =>
            new ElevatorStatus
            (_elevator.FloorNumber,
                _elevator.Direction,
                _elevator.DoorStatus,
                _elevator.IsEnabled);

        private void SetElevatorObservability()
        {
            _elevator.DirectionChangedEvent += OnDirectionChangedEvent;
            _elevator.DoorStateChangedEvent += OnDoorStateChangedEvent;
            _elevator.FloorNumberChangedEvent += OnFloorNumberChangedEvent;
        }

        private void OnDirectionChangedEvent(object sender, ElevatorDirectionChangedEventArgs e)
        {
            //TODO: This may not be needed?
        }

        private void OnDoorStateChangedEvent(object sender, ElevatorDoorEventArgs e)
        {
            //TODO: This may not be needed?
        }

        private void OnFloorNumberChangedEvent(object sender, ElevatorFloorNumberChangedEventArgs e)
        {
            //TODO: This may not be needed?
        }
    }
}