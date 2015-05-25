using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using System.Threading;

namespace MDKControl.Core.Services
{
    public abstract class MoCoBusCommServiceBase : IMoCoBusCommService
    {
        public SemaphoreSlim _sendReceiveSemaphore = new SemaphoreSlim(1, 1);
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
            try
            {
                _sendReceiveSemaphore.Wait();

                ClearReceiveBuffer();

                Send(frame);
                return await ReceiveAsync().ConfigureAwait(false);
            }
            finally
            {
                _sendReceiveSemaphore.Release();
            }
        }

        public abstract Task<MoCoBusFrame> ReceiveAsync();

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
