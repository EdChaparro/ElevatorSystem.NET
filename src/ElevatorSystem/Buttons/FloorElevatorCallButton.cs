namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallButton : AbstractButton
    {
        public FloorElevatorCallButton(Direction direction)
        {
            Direction = direction;
        }

        public Direction Direction { get; }
    }
}