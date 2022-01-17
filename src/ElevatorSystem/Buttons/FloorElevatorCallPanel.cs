using System;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallPanel : AbstractPanel
    {
        public FloorElevatorCallPanel(bool hasDownButton, bool hasUpButton)
        {
            if (hasDownButton)
            {
                DownButton = new FloorElevatorCallButton(Direction.Down);
                Add(DownButton);
            }

            if (hasUpButton)
            {
                UpButton = new FloorElevatorCallButton(Direction.Up);
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