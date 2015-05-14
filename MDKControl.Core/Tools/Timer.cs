using System;
using System.Threading;
using System.Threading.Tasks;

namespace MDKControl.Tools
{
    public delegate void TimerCallback(object state);

    public sealed class Timer : CancellationTokenSource, IDisposable
    {
        public Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            Task.Delay(dueTime, Token).ContinueWith(async (t, s) =>
                {
                    var tuple = (Tuple<TimerCallback, object>) s;

                    while (true)
                    {
                        if (IsCancellationRequested)
                            break;
                        await Task.Delay(period);
                        Task.Run(() => tuple.Item1(tuple.Item2));
                    }

                },
                Tuple.Create(callback, state), 
                CancellationToken.None, 
                (TaskContinuationOptions)(TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion),
                TaskScheduler.Default);
        }

        public new void Dispose() { base.Cancel(); }
    }
}
