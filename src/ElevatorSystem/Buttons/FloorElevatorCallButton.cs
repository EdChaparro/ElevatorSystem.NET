using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallButton : AbstractButton
    {
        public FloorElevatorCallButton(int floorNbr, Direction direction)
        {
            FloorNumber = floorNbr;
            Direction = direction;
        }

        public int FloorNumber { get; }

        public Direction Direction { get; }
    }
}