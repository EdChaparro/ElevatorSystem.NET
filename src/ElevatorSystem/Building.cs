using IntrepidProducts.Common;
using IntrepidProducts.ElevatorSystem.Banks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem
{
    public class Buildings : List<Building>
    { }

    public class Building : AbstractEntity, IHasFloors
    {
        public Building()   //Parameter-less constructor added to support serialization
        { }

        public Building(params IBank[] banks)
        {
            var result = Add(banks);

            if (result == false)
            {
                throw new ArgumentException("Invalid elevator bank set specified");
            }
        }

        public string? Name { get; set; }

        private readonly Dictionary<Guid, IBank> _banks = new Dictionary<Guid, IBank>();

        #region Banks
        public bool Add(params IBank[] banks)
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

        public IEnumerable<IBank> Banks => _banks.Values.ToList();

        public IBank? GetBank(Guid bankId)
        {
            if (!_banks.ContainsKey(bankId))
            {
                return null;
            }

            return _banks[bankId];
        }

        public IBank? GetBank(string bankName)
        {
            return _banks.Values.FirstOrDefault(x => x.Name == bankName);
        }
        #endregion

        #region IHasFloor
        public int NumberOfFloors => OrderedFloorNumbers.DefaultIfEmpty(0).Max();

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

        //Using DefaultIfEmpty to prevent exceptions on serialization
        public int LowestFloorNbr => OrderedFloorNumbers.DefaultIfEmpty(0).Min();
        public int HighestFloorNbr => OrderedFloorNumbers.DefaultIfEmpty(0).Max();
        #endregion

        public override string ToString()
        {
            return $"Number of Banks: {NumberOfBanks}";
        }
    }
}