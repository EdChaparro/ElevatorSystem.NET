using System;
using System.Threading;

namespace IntrepidProducts.ElevatorSystem.Service
{
    public interface IEngine
    {
        void Start();
        void Stop();
    }

    public abstract class AbstractEngine : IEngine
    {
        protected AbstractEngine()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
        }

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

                Thread.Sleep(SleepIntervalInMilliseconds);
            }
        }

        protected abstract void DoEngineLoop();


        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Engine is not running");
            }

            CancellationTokenSource.Cancel();
            IsStopRequested = true;
            IsRunning = false;
        }


    }
}