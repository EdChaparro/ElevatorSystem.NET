using System;
using System.Reflection;

namespace IntrepidProducts.ElevatorSystem
{
    public interface IHasId
    {
        Guid Id { get; set; }
    }

    public abstract class AbstractEntity : IHasId
    {
        protected AbstractEntity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }

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
    }
}