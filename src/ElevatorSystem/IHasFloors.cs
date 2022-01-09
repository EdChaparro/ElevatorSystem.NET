using System.Collections.Generic;

namespace IntrepidProducts.ElevatorSystem
{
    public interface IHasFloors
    {
        int NumberOfFloors { get; }
        IEnumerable<int> OrderedFloorNumbers { get; }

        int LowestFloorNbr { get; }
        int HighestFloorNbr { get; }
    }
}