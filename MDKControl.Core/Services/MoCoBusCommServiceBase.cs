using System;
using System.Threading;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using Nito.AsyncEx;
using System.Diagnostics;

namespace MDKControl.Core.Services
{
    public abstract class MoCoBusCommServiceBase : IMoCoBusCommService
    {
        private readonly AsyncLock _mutex = new AsyncLock();

        private ConnectionState _connectionState = ConnectionState.Disconnected;

        public event EventHandler ConnectionChanged;
        public event EventHandler DataReceived;

        public abstract void Connect();

        public abstract void Disconnect();

        public virtual ConnectionState ConnectionState
        {
            get { return _connectionState; }
            protected set
            {
                _connectionState = value;
                OnConnectionChanged();
            }
        }

        public virtual bool IsConnected
        {
            get { return ConnectionState == ConnectionState.Connected; }
        }

        public abstract void Send(MoCoBusFrame frame);

        public abstract void ClearReceiveBuffer();

        public virtual async Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));
            return await SendAndReceiveAsync(frame, cts.Token).ConfigureAwait(false);
        }

        public virtual async Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame, CancellationToken token)
        {
            using (await _mutex.LockAsync(token).ConfigureAwait(false))
            {
                for (var retry = 0; retry < 3; retry++)
                {
                    try
                    {
                        ClearReceiveBuffer();

                        Send(frame);
                        return await ReceiveAsync(token).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine("{0}: SendAndReceiveAsync: Timeout!", DateTime.UtcNow);
                    }

                    //await Task.Delay(50).ConfigureAwait(false);
                    Debug.WriteLine("{0}: SendAndReceiveAsync: Retry!", DateTime.UtcNow);
                }
            }

            throw new TimeoutException();
        }

        public abstract Task<MoCoBusFrame> ReceiveAsync(CancellationToken token);

        protected virtual void OnConnectionChanged()
        {
            var handler = ConnectionChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDataReceived()
        {
            var handler = DataReceived;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
