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

        public bool RequestStopAtFloorNumber(int floorNbr)
        {
            if (!_elevator.IsEnabled)
            {
                return false;
            }

            //TODO: Modify to simulate elevator movement latency
            return _elevator.RequestStopAtFloorNumber(floorNbr);
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