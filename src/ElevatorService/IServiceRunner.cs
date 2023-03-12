namespace IntrepidProducts.ElevatorService
{
    public interface IServiceRunner<in TEntity> where TEntity : class
    {
        bool Start(TEntity entity);
        int Count { get; }
        Task<bool> StopAsync(TEntity entity);
        bool IsRunning(TEntity entity);
    }
}