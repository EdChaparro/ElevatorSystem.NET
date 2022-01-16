using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBank : IHasId, IHasFloors
    {
        bool Add(params IElevator[] elevators);
        int NumberOfElevators { get; }
    }

    public class Bank : AbstractEntity, IBank
    {
        public Bank(params IFloor[] floors)
        {
            var result = Add(floors);

            if (result == false)
            {
                throw new ArgumentException("Invalid floor set specified");
            }
        }

        private readonly Dictionary<Guid, IElevator> _elevators = new Dictionary<Guid, IElevator>();
        private readonly Dictionary<int, IFloor> _floors = new Dictionary<int, IFloor>();

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
        private bool Add(params IFloor[] floors)
        {
            var itemsToAdd = new Dictionary<int, IFloor>();

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

            return true;
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