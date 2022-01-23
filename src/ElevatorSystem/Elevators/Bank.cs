using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class Bank : AbstractEntity
    {
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

        #region Elevators
        private void AddElevators(int nbrOfElevators)
        {
            var itemsToAdd = new Dictionary<Guid, IElevatorCommandAdapter>();

            for (int i = 0; i < nbrOfElevators; i++)
            {

                var eAdapter = new FauxElevatorCommandAdapter
                                    (new Elevator(OrderedFloorNumbers.ToArray()));
                itemsToAdd[eAdapter.ElevatorId] = eAdapter;
            }

            itemsToAdd.ToList().ForEach
                (x => _elevatorCommandAdapters.Add(x.Key, x.Value));
        }

        public int NumberOfElevators => _elevatorCommandAdapters.Count;
        #endregion

        #region Floors
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
                    floor.Panel = new FloorElevatorCallPanel(false, true);
                    continue;
                }

                if (floor.Equals(highestFloor))
                {
                    floor.Panel = new FloorElevatorCallPanel(true, false);
                    continue;
                }

                floor.Panel = new FloorElevatorCallPanel(true, true);
            }
        }
        #endregion

        #region IHasFloor
        public int NumberOfFloors => _floors.Count;
        public IEnumerable<int> OrderedFloorNumbers => _floors.Keys.OrderBy(x => x);

        public int LowestFloorNbr => OrderedFloorNumbers.Any() ? OrderedFloorNumbers.Min() : 0;
        public int HighestFloorNbr => OrderedFloorNumbers.Any() ? OrderedFloorNumbers.Max() : 0;
        #endregion
        public override string ToString()
        {
            return $"{NumberOfFloors} serviced by {NumberOfElevators} Elevators";
        }
    }
}