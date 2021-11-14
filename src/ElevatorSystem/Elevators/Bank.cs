using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBank
    {
        bool Add(params IElevator[] elevators);
        int NumberOfElevators { get; }
    }

    public class Bank : IBank
    {
        private readonly Dictionary<Guid, IElevator> _elevators = new Dictionary<Guid, IElevator>();

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

        public override string ToString()
        {
            return $"Number of Elevators: {NumberOfElevators}";
        }
    }
}