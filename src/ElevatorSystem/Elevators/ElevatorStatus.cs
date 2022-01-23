namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorStatus
    {
        public ElevatorStatus(Direction direction = Direction.Stationary,
            DoorStatus doorStatus = DoorStatus.Closed,
            int? currentFloorNumber = null,
            bool isEnabled = true)
        {
            Direction = direction;
            DoorStatus = doorStatus;
            CurrentFloorNumber = currentFloorNumber;
            IsEnabled = isEnabled;
        }

        public Direction Direction { get; } = Direction.Stationary;
        public DoorStatus DoorStatus { get; } = DoorStatus.Closed;
        public bool IsEnabled { get; } = true;
        public int? CurrentFloorNumber { get; }

        public bool IsMoving => Direction != Direction.Stationary;
        public bool IsMovingUp => Direction == Direction.Up;
        public bool IsMovingDown => Direction == Direction.Down;

        public bool IsDoorOpen => DoorStatus == DoorStatus.Open;
        public bool IsDoorClosed => DoorStatus == DoorStatus.Closed;
    }
}