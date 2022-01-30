namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorStatus
    {
        public ElevatorStatus(int currentFloorNumber,
            Direction direction = Direction.Up,
            DoorStatus doorStatus = DoorStatus.Closed,
            bool isEnabled = true)
        {
            CurrentFloorNumber = currentFloorNumber;
            Direction = direction;
            DoorStatus = doorStatus;
            IsEnabled = isEnabled;
        }

        public Direction Direction { get; }
        public DoorStatus DoorStatus { get; }
        public bool IsEnabled { get; } = true;
        public int CurrentFloorNumber { get; }

        //Elevators are considered to be constantly in motion, so long as the door is closed.
        public bool IsMoving => DoorStatus == DoorStatus.Closed;
        public bool IsMovingUp => (IsMoving && Direction == Direction.Up);
        public bool IsMovingDown => (IsMoving && Direction == Direction.Down);

        public bool IsDoorOpen => DoorStatus == DoorStatus.Open;
        public bool IsDoorClosed => DoorStatus == DoorStatus.Closed;
    }
}