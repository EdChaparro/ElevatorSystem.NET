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
            return _elevator.Direction switch
            {
                Elevators.Direction.Down => FloorNbr < _elevator.CurrentFloorNumber,
                Elevators.Direction.Up => FloorNbr > _elevator.CurrentFloorNumber,
                _ => false //Should never drop to here
            };
        }
    }
}