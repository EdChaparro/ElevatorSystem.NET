using System;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallPanel : AbstractPanel
    {
        public FloorElevatorCallPanel(bool hasDownButton, bool hasUpButton)
        {
            if (hasDownButton)
            {
                DownButton = new FloorButton(Direction.Down);
                Add(DownButton);
            }

            if (hasUpButton)
            {
                UpButton = new FloorButton(Direction.Up);
                Add(UpButton);
            }

            if (NumberOfButtons == 0)
            {
                throw new ArgumentException("Floor Panel must have at least one button");
            }
        }

        public FloorButton? DownButton { get; }
        public FloorButton? UpButton { get; }
    }
}