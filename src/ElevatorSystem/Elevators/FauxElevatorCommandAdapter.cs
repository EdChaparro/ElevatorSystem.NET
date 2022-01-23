using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class FauxElevatorCommandAdapter : IElevatorCommandAdapter
    {
        public FauxElevatorCommandAdapter(Elevator elevator, 
            int moveLatencyInMilliseconds = 1000)
        {
            _elevator = elevator;
            _status = new ElevatorStatus();

            SetElevatorObservability();
        }

        private readonly Elevator _elevator;
        private readonly ElevatorStatus _status;

        public Guid ElevatorId => _elevator.Id;

        public bool StopAt(int floorNbr)
        {
            return false;   //TODO: Finish me
        }

        public ElevatorStatus Status =>
            new ElevatorStatus
            (_elevator.Direction, _elevator.DoorStatus,
                _elevator.FloorNumber, _elevator.IsEnabled);

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