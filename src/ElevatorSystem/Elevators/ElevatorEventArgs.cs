using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorEventArgs : EventArgs
    {
        public ElevatorEventArgs(Guid elevatorId)
        {
            ElevatorId = elevatorId;
        }

        public Guid ElevatorId { get; }
    }
}