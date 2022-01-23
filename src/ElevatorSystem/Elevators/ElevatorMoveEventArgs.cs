using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorMoveEventArgs : ElevatorEventArgs
    {
        public ElevatorMoveEventArgs(Guid elevatorId, Direction direction, int floorNbr)
            : base(elevatorId)
        {
            Direction = direction;
            CurrentFloorNbr = floorNbr;
        }

        public Direction Direction { get; }
        public int CurrentFloorNbr { get; }
    }
}