using System;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallPanel : AbstractPanel<FloorElevatorCallButton>
    {
        public FloorElevatorCallPanel(Bank bank, bool hasDownButton, bool hasUpButton)
        {
            if (hasDownButton)
            {
                DownButton = new FloorElevatorCallButton(bank, Direction.Down);
                Add(DownButton);
            }

            if (hasUpButton)
            {
                UpButton = new FloorElevatorCallButton(bank, Direction.Up);
                Add(UpButton);
            }

            if (NumberOfButtons == 0)
            {
                throw new ArgumentException("Floor Panel must have at least one button");
            }
        }

        public FloorElevatorCallButton? DownButton { get; }
        public FloorElevatorCallButton? UpButton { get; }
    }
}