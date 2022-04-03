using IntrepidProducts.ElevatorSystem.Elevators;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Banks
{
    /// <summary>
    /// Service loop intended to run in an independent thread.
    /// </summary>
    public class BankEngine : AbstractEngine
    {
        public BankEngine(Bank bank)
        {
            SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;

            Bank = bank;
            Strategy = new IdleStrategy();  //TODO: use IoC
        }

        private Bank Bank { get; }
        private IStrategy Strategy { get; }

        protected override void DoEngineLoop()
        {
            Strategy.AssignElevators(Bank);
        }
    }
}