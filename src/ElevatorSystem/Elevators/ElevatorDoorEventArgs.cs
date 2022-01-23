using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class ElevatorDoorEventArgs : ElevatorEventArgs
    {
        public ElevatorDoorEventArgs(Guid elevatorId, DoorStatus doorStatus)
            : base(elevatorId)
        {
            DoorStatus = doorStatus;
        }

        public DoorStatus DoorStatus { get; }
    }
}