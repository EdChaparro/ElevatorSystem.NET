namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevatorCommandAdapter
    {
        bool StopAt(int floorNbr);
        ElevatorStatus Status { get; }
    }
}