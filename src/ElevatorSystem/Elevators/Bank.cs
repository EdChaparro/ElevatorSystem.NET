using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBank : IHasId, IHasFloors
    {
        bool Add(params IElevator[] elevators);
        int NumberOfElevators { get; }
    }

    public class Bank : AbstractEntity, IBank
    {
        public Bank(params Floor[] floors)
        {
            var result = Add(floors);

            if (result == false)
            {
                throw new ArgumentException("Invalid floor set specified");
            }
        }

        private readonly Dictionary<Guid, IElevator> _elevators = new Dictionary<Guid, IElevator>();
        private readonly SortedDictionary<int, Floor> _floors = new SortedDictionary<int, Floor>();

        #region Elevators
        public bool Add(params IElevator[] elevators)
        {
            var itemsToAdd = new Dictionary<Guid, IElevator>();

            foreach (var elevator in elevators)
            {
                if (_elevators.ContainsKey(elevator.Id))
                {
                    return false;
                }

                if (itemsToAdd.ContainsKey(elevator.Id))
                {
                    return false;
                }

                itemsToAdd[elevator.Id] = elevator;
            }

            itemsToAdd.ToList().ForEach
                (x => _elevators.Add(x.Key, x.Value));

            return true;
        }

        public int NumberOfElevators => _elevators.Count;
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
                    floor.Panel = new FloorPanel(false, true);
                    continue;
                }

                if (floor.Equals(highestFloor))
                {
                    floor.Panel = new FloorPanel(true, false);
                    continue;
                }

                floor.Panel = new FloorPanel(true, true);
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