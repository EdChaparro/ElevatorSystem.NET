using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Elevator : AbstractEntity
    {
        public Elevator(params int[] floorNbrs)
        {
            _floorNbrs = floorNbrs;
            FloorRequestPanel = new ElevatorFloorRequestPanel(this);

            FloorNumber = _floorNbrs.Min();
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

                if (_doorStatus == DoorStatus.Open)
                {
                    var floorRequestButton = FloorRequestPanel.GetButtonForFloorNumber(FloorNumber);

                    if (floorRequestButton != null)
                    {
                        floorRequestButton.SetPressedTo(false);
                    }
                }

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
        private Direction _direction = Direction.Up;

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

        private readonly int[] _floorNbrs;
        public IEnumerable<int> OrderedFloorNumbers => _floorNbrs.OrderBy(x => x);

        private int _floorNumber;

        public int FloorNumber
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

        private void RaiseFloorNumberChangedEvent(int floorNumber)
        {
            FloorNumberChangedEvent?.Invoke(this,
                new ElevatorFloorNumberChangedEventArgs(Id, floorNumber));
        }
        #endregion
    }
}