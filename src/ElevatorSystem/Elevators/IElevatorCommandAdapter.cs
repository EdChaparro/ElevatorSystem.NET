using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorCommandAdapter
    {
        Guid ElevatorId { get; }
        bool RequestStopAtFloorNumber(int floorNbr);
        public void SetEnabledState(bool isEnabled);

        ElevatorStatus Status { get; }
    }
}