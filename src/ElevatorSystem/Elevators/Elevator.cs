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
            FloorRequestPanel.PanelButtonPressedEvent += OnPanelButtonPressedEvent;

            CurrentFloorNumber = OrderedFloorNumbers.Min();
        }

        private readonly HashSet<int> _requestedFloorStops = new HashSet<int>();

        public IEnumerable<int> RequestedFloorStops => _requestedFloorStops.OrderBy(x => x);

        private void OnPanelButtonPressedEvent(object sender, PanelButtonPressedEventArgs<ElevatorFloorRequestButton> e)
        {
            _requestedFloorStops.Add(e.Button.FloorNbr);
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
                        _requestedFloorStops.Remove(floorRequestButton.FloorNbr);
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

            private set
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

        private void SetDirectionToStopAt(int floorNbr)
        {
            Direction = (floorNbr < CurrentFloorNumber) ? Direction.Down : Direction.Up;
        }
        #endregion

        #region Floor
        public readonly IEnumerable<int> OrderedFloorNumbers;

        private int _currentFloorNumber;

        public int CurrentFloorNumber
        {
            get => _currentFloorNumber;

            set
            {
                if (value == OrderedFloorNumbers.Min())
                {
                    Direction = Direction.Up;
                }

                if (value == OrderedFloorNumbers.Max())
                {
                    Direction = Direction.Down;
                }

                _currentFloorNumber = value;
            }
        }

        public bool RequestStopAtFloorNumber(int value)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (value == CurrentFloorNumber)
            {
                if (DoorStatus == DoorStatus.Closed)
                {
                    DoorStatus = DoorStatus.Open;
                    return true;
                }

                return false;
            }

            var isValidFloorNumber = OrderedFloorNumbers.Any(x => x == value);

            if (isValidFloorNumber)
            {
                _requestedFloorStops.Add(value);
                NavigateToNextFloorStop();
                return true;
            }

            return false;
        }

        private void NavigateToNextFloorStop()
        {
            if (!RequestedFloorStops.Any())
            {
                return;  //Nothing to do
            }

            if (DoorStatus == DoorStatus.Open)
            {
                DoorStatus = DoorStatus.Closed;
            }

            var currentFloorNumber = CurrentFloorNumber;

            switch (Direction)
            {
                case Direction.Down:
                    if (currentFloorNumber != OrderedFloorNumbers.Min())
                    {
                        currentFloorNumber--;
                    }
                    break;
                case Direction.Up:
                    if (currentFloorNumber != OrderedFloorNumbers.Max())
                    {
                        currentFloorNumber++;
                    }
                    break;
            }

            if (currentFloorNumber == CurrentFloorNumber)
            {
                return; //Reached termination point
            }

            SetDirectionToStopAt(currentFloorNumber);
            CurrentFloorNumber = currentFloorNumber;
            RaiseFloorNumberChangedEvent(CurrentFloorNumber);
            if (RequestedFloorStops.Contains(CurrentFloorNumber))
            {
                DoorStatus = DoorStatus.Open;
                return;
            }

            NavigateToNextFloorStop();  //Recursive call
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