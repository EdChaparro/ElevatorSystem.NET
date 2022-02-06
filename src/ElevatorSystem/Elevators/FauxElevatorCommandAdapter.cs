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

            _elevator.Direction = _elevator.CurrentFloorNumber < floorNbr
                ? Direction.Up
                : Direction.Down;
        }

        public bool StopAt(int floorNbr)
        {
            if (!_elevator.IsEnabled)
            {
                return false;
            }

            _elevator.DoorStatus = DoorStatus.Closed;

            SetDirectionToStopAt(floorNbr);

            //TODO: Modify to simulate elevator movement latency
            if (_elevator.MoveToFloorNumber(floorNbr))
            {
                _elevator.DoorStatus = DoorStatus.Open;
                return true;
            }

            return false;
        }

        public void SetEnabledState(bool isEnabled)
        {
            _elevator.IsEnabled = isEnabled;
        }

        public ElevatorStatus Status =>
            new ElevatorStatus
            (_elevator.CurrentFloorNumber,
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