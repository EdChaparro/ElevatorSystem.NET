﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Elevator : AbstractEntity, IEngine
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

        private readonly HashSet<int> _requestedFloorStops = new HashSet<int>();

        public IEnumerable<int> RequestedFloorStops => _requestedFloorStops.OrderBy(x => x);

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

                    _requestedFloorStops.Remove(CurrentFloorNumber);
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

        private void SetDirectionToStopAt(int floorNbr)
        {
            Direction = (floorNbr < CurrentFloorNumber) ? Direction.Down : Direction.Up;
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
                if (RequestedFloorStops.Contains(_currentFloorNumber))
                {
                    DoorStatus = DoorStatus.Open;
                }
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
                Name = "TrafficLightTimerThread"
            };

            _elevatorEngineThread.Start();
        }

        public void Stop()
        {
            _elevatorEngine.Stop();

            if (_elevatorEngineThread != null && _elevatorEngineThread.IsAlive)
            {
                _elevatorEngineThread.Join();        //Wait for shutdown to complete
            }
        }
    }
}