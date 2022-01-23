using System;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class FauxElevatorCommandAdapter : IElevatorCommandAdapter
    {
        public FauxElevatorCommandAdapter(Elevator elevator, 
            int moveLatencyInMilliseconds = 1000)
        {
            _elevator = elevator;
            _status = new ElevatorStatus();
        }

        private readonly Elevator _elevator;
        private readonly ElevatorStatus _status;

        public Guid ElevatorId => _elevator.Id;

        public bool StopAt(int floorNbr)
        {
            return false;   //TODO: Finish me
        }

        public ElevatorStatus Status => _status;
    }
}