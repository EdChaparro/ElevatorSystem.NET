using IntrepidProducts.Common;

namespace IntrepidProducts.ElevatorService;

public interface IEntityBackgroundService<out TEntity> : IBackgroundService
    where TEntity : AbstractEntity
{
    TEntity Entity { get; }
}