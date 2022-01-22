using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{

    public interface IElevator : IHasId
    {
        string? Name { get; set; }

        //TODO: Add Weight Capacity

        ElevatorFloorRequestPanel FloorRequestPanel { get; }
    }

    public class Elevator : AbstractEntity, IElevator
    {
        public Elevator(params int[] floorNbrs)
        {
            FloorRequestPanel = new ElevatorFloorRequestPanel(floorNbrs);
        }

        public ElevatorFloorRequestPanel FloorRequestPanel { get; }

        public string? Name { get; set; }
    }
}