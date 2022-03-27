using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IElevator : IHasId, IEngine
    { }  //Facilitates Mocking

    public class Elevator : AbstractEntity, IElevator
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
            _elevatorEngine = new ElevatorEngine(this);
        }

        public bool IsOnAdministrativeLock { get; set; }

        private readonly HashSet<RequestedFloorStop> _requestedFloorStops = new HashSet<RequestedFloorStop>();

        private void RemoveRequestedFloorStop(int floorNbr)
        {
            var requestedFloorStop = _requestedFloorStops
                .FirstOrDefault(x => x.FloorNbr == floorNbr);

            if (requestedFloorStop != null)
            {
                _requestedFloorStops.Remove(requestedFloorStop);
            }
        }

        private void AddRequestedFloorStop(int floorNbr, Direction? direction = null)
        {
            var rfs = RequestedFloorStop.CreateRequestedFloorStop(floorNbr, direction);
            _requestedFloorStops.Add(rfs);
        }

        public IEnumerable<RequestedFloorStop> RequestedFloorStops =>
            _requestedFloorStops.OrderBy(x => x.FloorNbr);

        public bool IsIdle => IsEnabled && !RequestedFloorStops.Any();

        public bool IsStoppingAtFloorFromDirection(int floorNbr, Direction direction)
        {
            return Direction == direction && RequestedFloorStops
                .Any(x => x.FloorNbr == floorNbr);
        }

        private void OnPanelButtonPressedEvent(object sender, PanelButtonPressedEventArgs<ElevatorFloorRequestButton> e)
        {
            RequestStopAtFloorNumber(e.Button.FloorNbr);
        }

        public ElevatorFloorRequestPanel FloorRequestPanel { get; }

        public string? Name { get; set; }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                _isEnabled = value;
                FloorRequestPanel.IsEnabled = value;
            }
        }

        #region Door
        private DoorStatus _doorStatus = DoorStatus.Closed;

        //TODO: Make Doors Close automatically
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
                    IsOnAdministrativeLock = false;   //Release administrative lock
                    var floorRequestButton = FloorRequestPanel.GetButtonForFloorNumber(CurrentFloorNumber);

                    if (floorRequestButton != null)
                    {
                        floorRequestButton.SetPressedTo(false);
                    }

                    RemoveRequestedFloorStop(CurrentFloorNumber);
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

        public bool PressButtonForFloorNumber(int value)
        {
            var button = FloorRequestPanel.GetButtonForFloorNumber(value);

            return button?.SetPressedTo(true) ?? false;
        }

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

                RaiseFloorNumberChangedEvent(_currentFloorNumber);
                if (RequestedFloorStops.Any(x => x.FloorNbr == _currentFloorNumber))
                {
                    DoorStatus = DoorStatus.Open;
                }
            }
        }

        public bool RequestStopAtFloorNumber(int value, bool withAdministrativeLock = false)
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

            DoorStatus = DoorStatus.Closed;

            var isValidFloorNumber = OrderedFloorNumbers.Any(x => x == value);

            if (isValidFloorNumber)
            {
                if (IsIdle)
                {
                    if (value < CurrentFloorNumber)
                    {
                        Direction = Direction.Down;
                    }

                    if (value > CurrentFloorNumber)
                    {
                        Direction = Direction.Up;
                    }
                }

                if (withAdministrativeLock)
                {
                    IsOnAdministrativeLock = true;
                }

                AddRequestedFloorStop(value);

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

        private Thread? _elevatorEngineThread;
        private readonly ElevatorEngine _elevatorEngine;
        public void Start()
        {
            _elevatorEngineThread = new Thread(_elevatorEngine.Start)
            {
                Name = "ElevatorEngineThread"
            };

            _elevatorEngineThread.Start();
        }

        public void Stop()
        {
            _elevatorEngine.Stop();

            if (_elevatorEngineThread is { IsAlive: true })
            {
                _elevatorEngineThread.Join();        //Wait for shutdown to complete
            }
        }
    }
}