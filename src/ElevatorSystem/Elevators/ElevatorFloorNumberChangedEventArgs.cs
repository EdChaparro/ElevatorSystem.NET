using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorFloorNumberChangedEventArgs : ElevatorEventArgs
    {
        public ElevatorFloorNumberChangedEventArgs
            (Guid elevatorId, int? floorNbr = null)
            : base(elevatorId)
        {
            CurrentFloorNbr = floorNbr;
        }

        public int? CurrentFloorNbr { get; }
    }
}