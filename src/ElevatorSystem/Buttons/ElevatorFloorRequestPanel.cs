﻿using IntrepidProducts.ElevatorSystem.Elevators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ElevatorFloorRequestPanel : AbstractPanel<ElevatorFloorRequestButton>
    {
        public ElevatorFloorRequestPanel(Elevator elevator)
        {
            var floorNbrs = elevator.OrderedFloorNumbers;

            ValidateFloorArguments(elevator.OrderedFloorNumbers);

            foreach (var nbr in floorNbrs)
            {
                Add(new ElevatorFloorRequestButton(elevator, nbr));
            }
        }

        public IEnumerable<int> RequestedFloorStops
            => Buttons.Where(x => x.IsPressed)
                .Select(x => x.FloorNbr);

        public ElevatorFloorRequestButton? GetButtonForFloorNumber(int nbr)
        {
            return Buttons.FirstOrDefault(x => x.FloorNbr == nbr);
        }

        private void ValidateFloorArguments(IEnumerable<int> floorNbrs)
        {
            var floorNumbers = floorNbrs.ToList();

            if (floorNumbers.Contains(0))
            {
                throw new ArgumentException
                    ("Floor Number can not be zero");
            }

            if (floorNumbers.Count() < 2)
            {
                throw new ArgumentException
                    ("Floor Request Panel must have at least two floors");
            }

            if (floorNumbers.Distinct().Count() != floorNumbers.Count())
            {
                throw new ArgumentException
                    ("Floor numbers must be unique");
            }
        }
    }
}