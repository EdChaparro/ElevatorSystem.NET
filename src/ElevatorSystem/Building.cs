﻿using IntrepidProducts.ElevatorSystem.Banks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem
{
    public class Buildings : List<Building>
    { }

    public class Building : AbstractEntity, IHasFloors
    {
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
            if (IsEngineStarted)
            {
                throw new InvalidOperationException
                    ("Elevator Bank Count changes not permitted while Engine is running");
            }

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

        #region Engine

        public bool IsEngineStarted { get; private set; }
        public void StartAllElevatorBanks()
        {
            if (IsEngineStarted)
            {
                throw new InvalidOperationException("Engine already started");
            }

            IsEngineStarted = true;

            foreach (var bank in _banks.Values)
            {
                bank.Start();
            }
        }

        public void StopAllElevatorBanks()
        {
            IsEngineStarted = false;

            foreach (var bank in _banks.Values)
            {
                bank.Stop();
            }
        }
        #endregion

        public override string ToString()
        {
            return $"Number of Banks: {NumberOfBanks}";
        }
    }
}