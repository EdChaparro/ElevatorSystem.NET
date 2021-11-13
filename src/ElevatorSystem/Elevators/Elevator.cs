namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorStatus
    {
        bool IsEnabled { get; set; }

        public bool IsMoving { get; }
        public bool IsMovingUp { get; }
        public bool IsMovingDown { get; }

        public bool IsDoorOpen { get; }
        public bool IsDoorClosed { get; }

        //TODO: Add Weight Capacity
    }


    public interface IElevator : IElevatorStatus
    {
        Direction Direction { get; set; }
        DoorStatus DoorStatus { get; set; }

        //TODO: Add Weight Capacity
    }

    public class Elevator : IElevator
    {
        public Direction Direction { get; set; } = Direction.Stationary;
        public DoorStatus DoorStatus { get; set; } = DoorStatus.Closed;
        public bool IsEnabled { get; set; } = true;

        public bool IsMoving => Direction != Direction.Stationary;
        public bool IsMovingUp => Direction == Direction.Up;
        public bool IsMovingDown => Direction == Direction.Down;

        public bool IsDoorOpen => DoorStatus == DoorStatus.Open;
        public bool IsDoorClosed => DoorStatus == DoorStatus.Closed;
    }
}