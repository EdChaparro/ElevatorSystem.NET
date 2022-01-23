namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorStatus
    {
        public Direction Direction { get; set; } = Direction.Stationary;
        public DoorStatus DoorStatus { get; set; } = DoorStatus.Closed;
        public bool IsEnabled { get; set; } = true;

        public bool IsMoving => Direction != Direction.Stationary;
        public bool IsMovingUp => Direction == Direction.Up;
        public bool IsMovingDown => Direction == Direction.Down;

        public bool IsDoorOpen => DoorStatus == DoorStatus.Open;
        public bool IsDoorClosed => DoorStatus == DoorStatus.Closed;

        public int? CurrentFloorNumber { get; }
    }
}