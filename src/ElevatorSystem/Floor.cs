using System;

namespace IntrepidProducts.ElevatorSystem
{
    public interface IFloor
    {
        int Number { get; }
        string Name { get; }
    }

    public class Floor : IFloor
    {
        public Floor(int number, string name = null)
        {
            Number = number;

            if (number < 1)
            {
                throw new ArgumentException("Floor number must be greater than 1");
            }

            Name = name ?? number.ToString();
        }

        public string Name { get; }
        public int Number { get; }
    }
}
