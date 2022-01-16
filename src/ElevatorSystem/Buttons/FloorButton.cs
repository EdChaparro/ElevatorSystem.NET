namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorButton : AbstractButton
    {
        public FloorButton(Direction direction)
        {
            Direction = direction;
        }

        public Direction Direction { get; }
    }
}