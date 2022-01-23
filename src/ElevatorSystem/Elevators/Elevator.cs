using System;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Elevator : AbstractEntity
    {
        public Elevator(params int[] floorNbrs)
        {
            FloorRequestPanel = new ElevatorFloorRequestPanel(floorNbrs);
        }

        public ElevatorFloorRequestPanel FloorRequestPanel { get; }

        public string? Name { get; set; }

        private DoorStatus _doorStatus = DoorStatus.Closed;

        public DoorStatus DoorStatus
        {
            get
            {
                return _doorStatus;
            }

            set
            {
                if (value == _doorStatus)
                {
                    return;
                }

                _doorStatus = value;
                RaiseButtonPressedEvent(value);
            }
        }

        public event EventHandler<ElevatorDoorEventArgs>? DoorEvent;

        private void RaiseButtonPressedEvent(DoorStatus doorStatus)
        {
            DoorEvent?.Invoke(this,
                new ElevatorDoorEventArgs(Id, doorStatus));
        }
    }
}