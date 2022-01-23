using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorCommandAdapter
    {
        Guid ElevatorId { get; }
        bool StopAt(int floorNbr);
        ElevatorStatus Status { get; }
    }
}