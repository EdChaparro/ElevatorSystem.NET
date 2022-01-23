using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorDirectionChangedEventArgs : ElevatorEventArgs
    {
        public ElevatorDirectionChangedEventArgs
            (Guid elevatorId, Direction direction, int? floorNbr = null)
            : base(elevatorId)
        {
            Direction = direction;
            CurrentFloorNbr = floorNbr;
        }

        public Direction Direction { get; }
        public int? CurrentFloorNbr { get; }
    }
}