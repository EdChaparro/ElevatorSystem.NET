using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBank : IHasId, IHasFloors, IEngine
    {}  //Facilitates Mocking

    public class Bank : AbstractEntity, IBank
    {
        public Bank(int nbrOfElevators, Range floorRange)
            : this(nbrOfElevators, Enumerable.Range
            (floorRange.Start.Value, floorRange.End.Value).ToArray())
        {}

        public Bank(int nbrOfElevators, params int[] floorsNbr)
        {
            var result = Add(floorsNbr);

            if (result == false)
            {
                throw new ArgumentException("Invalid floor set specified");
            }

            AddElevators(nbrOfElevators);
            SetFloorCallButtonObservability();

            _bankEngine = new BankEngine(this);
        }

        private readonly SortedDictionary<int, Floor> _floors = new SortedDictionary<int, Floor>();

        #region Elevators
        private readonly Dictionary<Guid, Elevator> _elevators =
            new Dictionary<Guid, Elevator>();

        public IEnumerable<Elevator> Elevators => _elevators.Values.ToList();

        private void AddElevators(int nbrOfElevators)
        {
            var itemsToAdd = new Dictionary<Guid, Elevator>();

            for (int i = 0; i < nbrOfElevators; i++)
            {
                var elevator = new Elevator(OrderedFloorNumbers.ToArray());
                itemsToAdd[elevator.Id] = elevator;
                SetObservabilityFor(elevator);
            }

            itemsToAdd.ToList().ForEach
                (x => _elevators.Add(x.Key, x.Value));
        }

        private void SetObservabilityFor(Elevator elevator)
        {
            elevator.DoorStateChangedEvent += OnDoorStateChangedEvent;
        }

        private void OnDoorStateChangedEvent(object sender, ElevatorDoorEventArgs e)
        {
            if (e.DoorStatus != DoorStatus.Open)
            {
                return;
            }

            var floorPanel = GetFloorElevatorCallPanelFor(e.FloorNumber);

            if (floorPanel != null) //Should never happen, makes compiler happy
            {
                floorPanel.ResetButtonForElevatorArrival(e.Direction);
            }

            if (!IsStopRequestedAt(e.FloorNumber))
            {
                return; //Nothing to do
            }

            var isDownServiceRequested = IsDownStopRequestedAt(e.FloorNumber);
            var isUpServiceRequested = IsUpStopRequestedAt(e.FloorNumber);

            switch (e.Direction)
            {
                case Direction.Down:
                    _requestedFloorStopsDown.Remove(e.FloorNumber);
                    break;
                case Direction.Up:
                    _requestedFloorStopsUp.Remove(e.FloorNumber);
                    break;
            }

            var elevator = Elevators
                .Where(x => x.Id == e.ElevatorId)
                .Select(x => x).FirstOrDefault();

            if (elevator == null)
            {
                return; //This shouldn't happen
            }

            if (isDownServiceRequested && isUpServiceRequested)
            {
                return; //Up & Down service requested, don't change present elevator direction
            }

            if (elevator.RequestedFloorStops.Any())
            {
                return; //More stops in route
            }

            if (isDownServiceRequested)
            {
                if (elevator.Direction == Direction.Up)
                {
                    elevator.Direction = Direction.Down;
                    _requestedFloorStopsDown.Remove(e.FloorNumber);
                }

                return;
            }

            if (elevator.Direction == Direction.Down)
            {
                elevator.Direction = Direction.Up;
                _requestedFloorStopsDown.Remove(e.FloorNumber);
            }
        }

        private void SetFloorCallButtonObservability()
        {
            foreach (var floorNbr in OrderedFloorNumbers)
            {
                var panel = GetFloorElevatorCallPanelFor(floorNbr);

                if (panel == null) //This should never happen
                {
                    throw new NullReferenceException
                        ($"Floor Panel not found for number {floorNbr}, Bank ID {Id}");
                }

                panel.PanelButtonPressedEvent += OnFloorElevatorCallButtonPressedEvent;
            }
        }

        private void OnFloorElevatorCallButtonPressedEvent(object sender, PanelButtonPressedEventArgs<FloorElevatorCallButton> e)
        {
            var button = e.Button;

            switch (button.Direction)
            {
                case Direction.Down:
                    _requestedFloorStopsDown.Add(button.FloorNumber);
                    break;
                case Direction.Up:
                    _requestedFloorStopsUp.Add(button.FloorNumber);
                    break;
            }
        }

        public int NumberOfElevators => _elevators.Count;
        #endregion

        #region Floors
        public FloorElevatorCallPanel? GetFloorElevatorCallPanelFor(int floorNbr)
        {
            return GetFloorFor(floorNbr)?.Panel;
        }

        public bool PressButtonForFloorNumber(int floorNbr, Direction direction)
        {
            var panel = GetFloorElevatorCallPanelFor(floorNbr);

            var button = direction == Direction.Down ? panel?.DownButton : panel?.UpButton;

            return button?.SetPressedTo(true) ?? false;
        }

        public bool SetFloorName(int floorNbr, string name)
        {
            var floor = GetFloorFor(floorNbr);

            if (floor == null)
            {
                return false;
            }

            floor.Name = name;
            return true;
        }

        public Floor? GetFloorFor(int floorNbr)
        {
            if (_floors.ContainsKey(floorNbr))
            {
                var floor = _floors[floorNbr];
                return floor;
            }

            return null;
        }

        private bool Add(params int[] floorNbrs)
        {
            if (floorNbrs.Length < 2)
            {
                return false;   //Must have at least two floors
            }

            var itemsToAdd = new Dictionary<int, Floor>();

            foreach (var floorNbr in floorNbrs)
            {
                if (_floors.ContainsKey(floorNbr))
                {
                    return false;
                }

                if (itemsToAdd.ContainsKey(floorNbr))
                {
                    return false;
                }

                itemsToAdd[floorNbr] = new Floor(floorNbr);
            }

            itemsToAdd.ToList().ForEach
                (x => _floors.Add(x.Key, x.Value));

            AddCallButtonsTo(_floors);
            return true;
        }

        private void AddCallButtonsTo(SortedDictionary<int, Floor> floors)
        {
            var lowestFloor = floors.First().Value;
            var highestFloor = floors.Last().Value;

            foreach (var floor in floors.Values)
            {
                if (floor.Equals(lowestFloor))
                {
                    floor.Panel = new FloorElevatorCallPanel
                        (floor.Number, false, true);
                    continue;
                }

                if (floor.Equals(highestFloor))
                {
                    floor.Panel = new FloorElevatorCallPanel
                        (floor.Number, true, false);
                    continue;
                }

                floor.Panel = new FloorElevatorCallPanel
                    (floor.Number, true, true);
            }
        }
        #endregion

        #region IHasFloor
        public int NumberOfFloors => _floors.Count;
        public IEnumerable<int> OrderedFloorNumbers => _floors.Keys.OrderBy(x => x);

        public int LowestFloorNbr => OrderedFloorNumbers.Any() ? OrderedFloorNumbers.Min() : 0;
        public int HighestFloorNbr => OrderedFloorNumbers.Any() ? OrderedFloorNumbers.Max() : 0;
        #endregion

        #region Requested Floor Stops
        private readonly HashSet<int> _requestedFloorStopsUp = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsUp => _requestedFloorStopsUp.OrderBy(x => x);

        private readonly HashSet<int> _requestedFloorStopsDown = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsDown => _requestedFloorStopsDown.OrderBy(x => x);

        public bool IsStopRequestedAt(int floorNumber)
        {
            return IsDownStopRequestedAt(floorNumber) ||
                   IsUpStopRequestedAt(floorNumber);
        }

        public bool IsDownStopRequestedAt(int floorNumber)
        {
            return _requestedFloorStopsDown.Contains(floorNumber);
        }

        public bool IsUpStopRequestedAt(int flrNbr)
        {
            return _requestedFloorStopsUp.Contains(flrNbr);
        }
        #endregion



        #region IEngine
        private Thread? _bankEngineThread;
        private readonly BankEngine _bankEngine;

        public void Start()
        {
            foreach (var elevator in Elevators)
            {
                elevator.Start();
                elevator.RequestStopAtFloorNumber(LowestFloorNbr);
            }

            _bankEngineThread = new Thread(_bankEngine.Start)
            {
                Name = "BankEngineThread"
            };

            _bankEngineThread.Start();
        }

        public void Stop()
        {
            foreach (var elevator in Elevators)
            {
                elevator.RequestStopAtFloorNumber(LowestFloorNbr);
                elevator.Stop();
            }

            _bankEngine.Stop();

            if (_bankEngineThread is { IsAlive: true })
            {
                _bankEngineThread.Join();        //Wait for shutdown to complete
            }
        }

        #endregion
        public override string ToString()
        {
            return $"{NumberOfFloors} serviced by {NumberOfElevators} Elevators";
        }
    }
}