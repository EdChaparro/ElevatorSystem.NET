using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Banks;

namespace IntrepidProducts.ElevatorSystem
{
    public class Building : IHasFloors
    {
        public Building(params IBank[] banks)
        {
            var result = Add(banks);

            if (result == false)
            {
                throw new ArgumentException("Invalid elevator bank set specified");
            }
        }

        private readonly Dictionary<Guid, IBank> _banks = new Dictionary<Guid, IBank>();

        private bool Add(params IBank[] banks)
        {
            var itemsToAdd = new Dictionary<Guid, IBank>();

            foreach (var bank in banks)
            {
                if (_banks.ContainsKey(bank.Id))
                {
                    return false;
                }

                if (itemsToAdd.ContainsKey(bank.Id))
                {
                    return false;
                }

                itemsToAdd[bank.Id] = bank;
            }

            itemsToAdd.ToList().ForEach
                (x => _banks.Add(x.Key, x.Value));

            return true;
        }

        public int NumberOfBanks => _banks.Count;

        #region IHasFloor
        public int NumberOfFloors => OrderedFloorNumbers.Max();

        public IEnumerable<int> OrderedFloorNumbers
        {
            get
            {
                var floorNbrs = new List<int>();

                foreach (var bank in _banks.Values)
                {
                    floorNbrs.AddRange(bank.OrderedFloorNumbers);
                }

                return floorNbrs.Distinct().OrderBy(x => x);
            }
        }

        public int LowestFloorNbr => OrderedFloorNumbers.Min();
        public int HighestFloorNbr => OrderedFloorNumbers.Max();
        #endregion

        public void StartAllElevatorBanks()
        {
            foreach (var bank in _banks.Values)
            {
                bank.Start();
            }
        }

        public void StopAllElevatorBanks()
        {
            foreach (var bank in _banks.Values)
            {
                bank.Stop();
            }
        }

        public override string ToString()
        {
            return $"Number of Banks: {NumberOfBanks}";
        }
    }
}