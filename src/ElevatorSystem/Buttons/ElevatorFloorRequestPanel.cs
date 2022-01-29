using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;

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

        public ElevatorFloorRequestButton? GetButtonForFloorNumber(int nbr)
        {
            return Buttons.FirstOrDefault(x => x.FloorNbr == nbr);
        }

        private void ValidateFloorArguments(IEnumerable<int> floorNbrs)
        {
            if (floorNbrs.Count() < 2)
            {
                throw new ArgumentException
                    ("Floor Request Panel must have at least two floors");
            }

            if (floorNbrs.Min() < 1)
            {
                throw new ArgumentException
                    ("Floor numbers must be positive");
            }

            if (floorNbrs.Distinct().Count() != floorNbrs.Count())
            {
                throw new ArgumentException
                    ("Floor numbers must be unique");
            }
        }
    }
}