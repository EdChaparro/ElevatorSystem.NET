using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ElevatorFloorRequestButton : AbstractButton
    {
        public ElevatorFloorRequestButton(Elevator elevator, int floorNbr)
        {
            _elevator = elevator;
            FloorNbr = floorNbr;
        }

        private readonly Elevator _elevator;
        public int FloorNbr { get; }

        protected override bool IsOkToProceedWithButtonPress()
        {
            switch (_elevator.Direction)
            {
                case Elevators.Direction.Down:
                    return FloorNbr < _elevator.FloorNumber;

                case Elevators.Direction.Up:
                    return FloorNbr > _elevator.FloorNumber;
            }

            return false;   //Should never drop to here
        }
    }
}