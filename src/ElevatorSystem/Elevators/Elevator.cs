using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Elevator : AbstractEntity
    {
        public Elevator(params int[] floorNbrs)
        {
            FloorRequestPanel = new ElevatorFloorRequestPanel(floorNbrs);
        }

        public ElevatorFloorRequestPanel FloorRequestPanel { get; }

        public string? Name { get; set; }
    }
}