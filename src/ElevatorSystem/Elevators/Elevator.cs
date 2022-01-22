namespace IntrepidProducts.ElevatorSystem.Elevators
{

    public interface IElevator : IHasId
    {
        string? Name { get; }

        //TODO: Add Weight Capacity
    }

    public class Elevator : AbstractEntity, IElevator
    {
        public Elevator(string? name = null)
        {
            Name = name;
        }
        public string? Name { get; }
    }
}