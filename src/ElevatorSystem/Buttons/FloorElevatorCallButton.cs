using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallButton : AbstractButton
    {
        public FloorElevatorCallButton(Bank bank, Direction direction)
        {
            _bank = bank;
            Direction = direction;
        }

        private readonly Bank _bank;

        public Direction Direction { get; }
    }
}