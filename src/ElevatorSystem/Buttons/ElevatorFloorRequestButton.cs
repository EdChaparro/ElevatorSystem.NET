namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ElevatorFloorRequestButton : AbstractButton
    {
        public ElevatorFloorRequestButton(int floorNbr)
        {
            FloorNbr = floorNbr;
        }

        public int FloorNbr { get; }
    }
}