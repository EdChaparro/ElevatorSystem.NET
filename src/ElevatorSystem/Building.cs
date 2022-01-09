using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem
{
    public interface IBuilding
    {
        int NumberOfBanks { get; }
    }

    public class Building : IBuilding
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

        public override string ToString()
        {
            return $"Number of Banks: {NumberOfBanks}";
        }
    }
}