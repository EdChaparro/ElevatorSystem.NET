using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorDirectionChangedEventArgs : ElevatorEventArgs
    {
        public ElevatorDirectionChangedEventArgs
            (Guid elevatorId, Direction direction)
            : base(elevatorId)
        {
            Direction = direction;
        }

        public Direction Direction { get; }
    }
}