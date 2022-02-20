using System;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class FloorElevatorCallPanel : AbstractPanel<FloorElevatorCallButton>
    {
        public FloorElevatorCallPanel(int floorNbr, bool hasDownButton, bool hasUpButton)
        {
            if (hasDownButton)
            {
                DownButton = new FloorElevatorCallButton(floorNbr, Direction.Down);
                Add(DownButton);
            }

            if (hasUpButton)
            {
                UpButton = new FloorElevatorCallButton(floorNbr, Direction.Up);
                Add(UpButton);
            }

            if (NumberOfButtons == 0)
            {
                throw new ArgumentException("Floor Panel must have at least one button");
            }
        }

        public void ResetButtonForElevatorArrival(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    if (DownButton != null)
                    {
                        DownButton.SetPressedTo(false);
                    }

                    break;

                case Direction.Up:
                    if (UpButton != null)
                    {
                        UpButton.SetPressedTo(false);
                    }

                    break;
            }
        }

        private bool HasDownButton => DownButton != null;
        private bool HasUpButton => UpButton != null;

        public FloorElevatorCallButton? DownButton { get; }
        public FloorElevatorCallButton? UpButton { get; }
    }
}