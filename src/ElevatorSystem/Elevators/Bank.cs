using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Bank : AbstractEntity, IHasFloors, IEngine
    {
        public Bank(int nbrOfElevators, Range floorRange) : this(nbrOfElevators,
            Enumerable.Range
                    (floorRange.Start.Value, floorRange.End.Value)
                .Select(x => new Floor(x)).ToArray())
        {}

        public Bank(int nbrOfElevators, params Floor[] floors)
        {
            var result = Add(floors);

            if (result == false)
            {
                throw new ArgumentException("Invalid floor set specified");
            }

            AddElevators(nbrOfElevators);
        }

        private readonly Dictionary<Guid, IElevatorCommandAdapter> _elevatorCommandAdapters =
            new Dictionary<Guid, IElevatorCommandAdapter>();
        private readonly SortedDictionary<int, Floor> _floors = new SortedDictionary<int, Floor>();

        public IEnumerable<IElevatorCommandAdapter> ElevatorCommandAdapters
            => _elevatorCommandAdapters.Values.ToList();

        #region Elevators

        public IEnumerable<ElevatorStatus> ElevatorStates
            => _elevatorCommandAdapters.Values.Select(x => x.Status);

        private void AddElevators(int nbrOfElevators)
        {
            var itemsToAdd = new Dictionary<Guid, IElevatorCommandAdapter>();

            for (int i = 0; i < nbrOfElevators; i++)
            {
                var elevator = new Elevator(OrderedFloorNumbers.ToArray());
                var eAdapter = new FauxElevatorCommandAdapter(this, elevator); //TODO: Use IoC
                itemsToAdd[eAdapter.ElevatorId] = eAdapter;
                SetObservabilityFor(elevator);
            }

            itemsToAdd.ToList().ForEach
                (x => _elevatorCommandAdapters.Add(x.Key, x.Value));
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

        public int NumberOfElevators => _elevatorCommandAdapters.Count;
        #endregion

        #region Floors
        public FloorElevatorCallPanel? GetFloorElevatorCallPanelFor(int floorNbr)
        {
            return GetFloor(floorNbr)?.Panel;
        }

        public bool SetFloorName(int floorNbr, string name)
        {
            var floor = GetFloor(floorNbr);

            if (floor == null)
            {
                return false;
            }

            floor.Name = name;
            return true;
        }

        private Floor? GetFloor(int floorNbr)
        {
            if (_floors.ContainsKey(floorNbr))
            {
                var floor = _floors[floorNbr];
                return floor;
            }

            return null;
        }

        private bool Add(params Floor[] floors)
        {
            if (floors.Length < 2)
            {
                return false;   //Must have at least two floors
            }

            var itemsToAdd = new Dictionary<int, Floor>();

            foreach (var floor in floors)
            {
                if (_floors.ContainsKey(floor.Number))
                {
                    return false;
                }

                if (itemsToAdd.ContainsKey(floor.Number))
                {
                    return false;
                }

                itemsToAdd[floor.Number] = floor;
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
                    floor.Panel = new FloorElevatorCallPanel(this, false, true);
                    continue;
                }

                if (floor.Equals(highestFloor))
                {
                    floor.Panel = new FloorElevatorCallPanel(this, true, false);
                    continue;
                }

                floor.Panel = new FloorElevatorCallPanel(this, true, true);
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
            SendAllElevatorsToHomeFloor();
        }

        public void Stop()
        {
            SendAllElevatorsToHomeFloor();
        }

        private void SendAllElevatorsToHomeFloor()
        {
            foreach (var adapter in ElevatorCommandAdapters)
            {
                adapter.StopAt(LowestFloorNbr);
            }
        }
        #endregion
        public override string ToString()
        {
            return $"{NumberOfFloors} serviced by {NumberOfElevators} Elevators";
        }
    }
}