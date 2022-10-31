using System;
using System.Reflection;
using IntrepidProducts.Common;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public class RequestedFloorStop
    {
        public RequestedFloorStop(int floorNbr, Direction? direction)
        {
            Id = Guid.NewGuid();
            FloorNbr = floorNbr;
            Direction = direction;
        }

        public Guid Id { get; }
        public int FloorNbr { get; }
        public Direction? Direction { get; }

        #region Equality
        public override bool Equals(object? obj)
        {
            Assembly.GetAssembly(typeof(string));
            return base.Equals(obj);
        }

        protected bool Equals(AbstractEntity other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion

        public static RequestedFloorStop CreateRequestedFloorStop(int floorNbr, Direction? direction = null)
        {
            var rfs = new RequestedFloorStop(floorNbr, direction);
            return rfs;
        }

        public override string ToString()
        {
            return $"Id: {Id}, FloorNbr: {FloorNbr}, Direction: {Direction}";
        }
    }
}