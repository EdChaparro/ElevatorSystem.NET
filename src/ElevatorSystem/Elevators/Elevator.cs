using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Elevator : AbstractEntity
    {
        public Elevator(Range floorRange) : this(Enumerable.Range
            (floorRange.Start.Value, floorRange.End.Value).ToArray())
        { }

        public Elevator(params int[] floorNbrs)
        {
            OrderedFloorNumbers = floorNbrs.OrderBy(x => x);
            FloorRequestPanel = new ElevatorFloorRequestPanel(this);

            MoveToFloorNumber(OrderedFloorNumbers.Min());
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
                    var floorRequestButton = FloorRequestPanel.GetButtonForFloorNumber(CurrentFloorNumber);

                    if (floorRequestButton != null)
                    {
                        floorRequestButton.SetPressedTo(false);
                    }
                }

                RaiseDoorStateChangedEvent(CurrentFloorNumber, Direction, value);
            }
        }

        public event EventHandler<ElevatorDoorEventArgs>? DoorStateChangedEvent;

        private void RaiseDoorStateChangedEvent
            (int floorNbr, Direction direction, DoorStatus doorStatus)
        {
            DoorStateChangedEvent?.Invoke(this,
                new ElevatorDoorEventArgs(Id, floorNbr, direction, doorStatus));
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
        public readonly IEnumerable<int> OrderedFloorNumbers;

        public int CurrentFloorNumber { get; private set; }
        public int NextStopFloorNumber { get; private set; }

        public bool MoveToFloorNumber(int value)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if ((value == CurrentFloorNumber) && (_doorStatus == DoorStatus.Open))
            {
                return false;
            }

            var isValidFloorNumber = OrderedFloorNumbers.Any(x => x == value);

            if (isValidFloorNumber)
            {
                NextStopFloorNumber = value;    //TODO: Eventually, this will not always match CurrentFloorNumber
                CurrentFloorNumber = value;
                RaiseFloorNumberChangedEvent(value);
                return true;
            }

            return false;
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