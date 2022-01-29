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

        public bool IsEnabled { get; set; } = true;

        #region Door
        private DoorStatus _doorStatus = DoorStatus.Closed;

        public DoorStatus DoorStatus
        {
            get => _doorStatus;

            set
            {
                if (value == _doorStatus)
                {
                    return;
                }

                _doorStatus = value;
                RaiseDoorStateChangedEvent(value);
            }
        }

        public event EventHandler<ElevatorDoorEventArgs>? DoorStateChangedEvent;

        private void RaiseDoorStateChangedEvent(DoorStatus doorStatus)
        {
            DoorStateChangedEvent?.Invoke(this,
                new ElevatorDoorEventArgs(Id, doorStatus));
        }
        #endregion

        #region Direction
        private Direction _direction = Direction.Stationary;

        public Direction Direction
        {
            get => _direction;

            set
            {
                if (value == _direction)
                {
                    return;
                }

                _direction = value;
                RaiseDirectionChangedEvent(value);
            }
        }

        public event EventHandler<ElevatorDirectionChangedEventArgs>? DirectionChangedEvent;

        private void RaiseDirectionChangedEvent(Direction direction)
        {
            DirectionChangedEvent?.Invoke(this,
                new ElevatorDirectionChangedEventArgs(Id, direction));
        }
        #endregion

        #region Floor
        private int? _floorNumber;

        public int? FloorNumber
        {
            get => _floorNumber;

            set
            {
                if (value == _floorNumber)
                {
                    return;
                }

                _floorNumber = value;
                RaiseFloorNumberChangedEvent(value);
            }
        }

        public event EventHandler<ElevatorFloorNumberChangedEventArgs>? FloorNumberChangedEvent;

        private void RaiseFloorNumberChangedEvent(int? floorNumber)
        {
            FloorNumberChangedEvent?.Invoke(this,
                new ElevatorFloorNumberChangedEventArgs(Id, floorNumber));
        }
        #endregion
    }
}