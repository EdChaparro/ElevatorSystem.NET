using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorCommandAdapter
    {
        Guid ElevatorId { get; }
        bool StopAt(int floorNbr);
        public void SetEnabledState(bool isEnabled);

        ElevatorStatus Status { get; }
    }
}