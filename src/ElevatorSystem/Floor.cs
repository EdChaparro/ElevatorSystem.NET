using System;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem
{
    public class Floor 
    {
        public Floor(int number)
        {
            Number = number;

            if (number == 0)
            {
                throw new ArgumentException("Floor number cannot be zero");
            }

            Name = number.ToString();
        }

        public string Name { get; set; }
        public int Number { get; }
        public bool IsLocked { get; set; }

        public FloorElevatorCallPanel? Panel { get; set; }

        #region Operators
        public static bool operator >(Floor floor1, Floor floor2)
        {
            return floor1.Number > floor2.Number;
        }

        public static bool operator <(Floor floor1, Floor floor2)
        {
            return floor1.Number < floor2.Number;
        }
        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            return obj is Floor floor && Equals(floor);
        }

        protected bool Equals(Floor other)
        {
            return Number.Equals(other.Number);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return $"Number: {Number}, Name: {Name}, IsLocked: {IsLocked}";
        }
    }
}