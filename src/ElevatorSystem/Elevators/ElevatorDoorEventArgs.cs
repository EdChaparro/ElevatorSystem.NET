using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorDoorEventArgs : ElevatorEventArgs
    {
        public ElevatorDoorEventArgs(Guid elevatorId,
            int floorNbr, Direction direction,  DoorStatus doorStatus)
            : base(elevatorId)
        {
            FloorNumber = floorNbr;
            Direction = direction;
            DoorStatus = doorStatus;
        }

        public int FloorNumber { get; }
        public Direction Direction { get; }
        public DoorStatus DoorStatus { get; }
    }
}