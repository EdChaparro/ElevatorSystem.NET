using System.Linq;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
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
        }

        private Bank Bank { get; }

        protected override void DoEngineLoop()
        {
            if (Bank.RequestedFloorStopsDown.Any())
            {
                ServiceDownRequests();
            }
        }

        private void ServiceDownRequests()
        {
            //TODO: Implement me
        }
    }
}