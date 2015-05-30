using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using System.Threading;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusCommService
    {
        event EventHandler ConnectionChanged;
        event EventHandler DataReceived;

        void Connect();
        void Disconnect();
        ConnectionState ConnectionState { get; }
        bool IsConnected { get; }

        void ClearReceiveBuffer();

        Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame);
        Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame, CancellationToken token);
        void Send(MoCoBusFrame frame);
        Task<MoCoBusFrame> ReceiveAsync();
        Task<MoCoBusFrame> ReceiveAsync(CancellationToken token);
    }
}
