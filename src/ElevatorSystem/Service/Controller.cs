using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorSystem.Service
{
    public interface IController : IEngine
    {}

    public class Controller : IController
    {
        public Controller(params Bank[] banks)
        {
            var result = Add(banks);

            if (result == false)
            {
                throw new ArgumentException("Invalid elevator bank set specified");
            }
        }

        private readonly Dictionary<Guid, Bank> _banks = new Dictionary<Guid, Bank>();

        private bool Add(params Bank[] banks)
        {
            var itemsToAdd = new Dictionary<Guid, Bank>();

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


        public void Start()
        {
            //TODO: Finish Me
        }

        public void Stop()
        {
            //TODO: Finish Me
        }
    }
}