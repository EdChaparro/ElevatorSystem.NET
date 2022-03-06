using System;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    /// <summary>
    /// Service loop intended to run in an independent thread.
    /// </summary>
    public class BankEngine : IEngine
    {
        public BankEngine(Bank bank)
        {
            SleepIntervalInMilliseconds = Configuration.EngineSleepIntervalInMilliseconds;

            Bank = bank;

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

        private Bank Bank { get; set; }

        public bool IsRunning { get; private set; }
        protected bool IsStopRequested { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; private set; }
        protected CancellationToken CancellationToken { get; private set; }

        protected int SleepIntervalInMilliseconds { get; set; }

        public void Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;

            IsRunning = true;

            while (IsStopRequested == false)
            {
                DoEngineLoop();
                bool isCancelled = CancellationToken.WaitHandle.WaitOne(SleepIntervalInMilliseconds);

                if (isCancelled)
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Elevator Engine is not running");
            }

            CancellationTokenSource.Cancel();
            IsStopRequested = true;
            IsRunning = false;
        }

        protected virtual void DoEngineLoop()
        {
            //TODO: Implement Bank Logic

            Thread.Sleep(SleepIntervalInMilliseconds);
        }
    }
}