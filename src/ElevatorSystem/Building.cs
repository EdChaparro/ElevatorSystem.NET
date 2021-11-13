using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem
{
    public interface IBuilding
    {
        bool Add(params IFloor[] floors);
        int NumberOfFloors { get; }
    }

    public class Building : IBuilding
    {
        private readonly Dictionary<int, IFloor> _floors = new Dictionary<int, IFloor>();

        public bool Add(params IFloor[] floors)
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

        public int NumberOfFloors => _floors.Count;
    }
}