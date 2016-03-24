using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using Nito.AsyncEx;
using Xamarin;

namespace MDKControl.Core.Services
{
    public abstract class MoCoBusCommServiceBase : IMoCoBusCommService
    {
        private const int MaxRetry = 3;

        private readonly AsyncLock _mutex = new AsyncLock();

        private int _retryCounter = 0;
        private ConnectionState _connectionState = ConnectionState.Disconnected;

        public event EventHandler ConnectionChanged;

        public abstract void Connect();

        public abstract void Disconnect();

        public virtual ConnectionState ConnectionState
        {
            get { return _connectionState; }
            protected set
            {
                _connectionState = value;
                if (_connectionState == ConnectionState.Connected)
                {
                    _retryCounter = 0;
                }
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
            return await SendAndReceiveAsync(frame, CancellationToken.None).ConfigureAwait(false);
        }

        public virtual async Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame, CancellationToken token)
        {
            using (await _mutex.LockAsync(token).ConfigureAwait(false))
            {
                for (var retry = 0; retry < MaxRetry; retry++)
                {
                    token.ThrowIfCancellationRequested();
                    
                    try
                    {
                        ClearReceiveBuffer();
                        Send(frame);
                        var result = await ReceiveAsync(CancellationToken.None).ConfigureAwait(false);
                        if (_retryCounter > 0) 
                        {
                            _retryCounter--;   
                        }
                        return result;
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine("SendAndReceiveAsync: Timeout! (Counter: {0})", _retryCounter);

                        if (_retryCounter < 1000)
                        {
                            _retryCounter += 5;
                        }
                        if (_retryCounter >= 100)
                        {
                            Debug.WriteLine("RetryCounter >= 100 ({0}): Try to reconnect!", _retryCounter);
                            Insights.Track("MoCoBusCommServiceBaseRetryCounterOverrun", 
                                new Dictionary<string, string> 
                                { 
                                    { "RetryCounter", _retryCounter.ToString() } 
                                });

                            _retryCounter = 0;
                            Task.Run(Disconnect);
                            await Task.Delay(500).ConfigureAwait(false);
                            break;
                        }
                    }

                    await Task.Delay(50).ConfigureAwait(false);
                    Debug.WriteLine("SendAndReceiveAsync: Retry!");
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
    }
}
