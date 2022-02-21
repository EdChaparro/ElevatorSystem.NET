using System;
using System.Collections.Generic;
using System.Linq;
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

            Controller = new BankController(this);
        }

        public IBankController Controller { get; }
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

        #region IEngine

        public void Start()
        {
            Controller.Start();
        }

        public void Stop()
        {
            Controller.Stop();
        }

        #endregion
        public override string ToString()
        {
            return $"{NumberOfFloors} serviced by {NumberOfElevators} Elevators";
        }
    }
}