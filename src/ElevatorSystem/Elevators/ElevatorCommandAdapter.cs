namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorCommandAdapter
    {
        bool StopAt(int floorNbr);
        ElevatorStatus Status { get; }
    }

    public class ElevatorCommandAdapter : IElevatorCommandAdapter
    {
        public ElevatorCommandAdapter(Elevator elevator)
        {
            _elevator = elevator;
            _status = new ElevatorStatus();
        }

        private Elevator _elevator;
        private ElevatorStatus _status;

        public bool StopAt(int floorNbr)
        {
            return false;   //TODO: Finish me
        }

        public ElevatorStatus Status => _status;
    }
}