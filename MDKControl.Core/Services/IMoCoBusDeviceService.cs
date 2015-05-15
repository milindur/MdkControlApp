using System;
using System.Threading.Tasks;
using MDKControl.Core.Models;

namespace MDKControl.Core.Services
{
    public interface IMoCoBusDeviceService
    {
        event EventHandler ConnectionChanged;
        event EventHandler DataReceived;

        void Connect();
        void Disconnect();
        ConnectionState ConnectionState { get; }
        bool IsConnected { get; }

        void Send(MoCoBusFrame frame);
        Task<MoCoBusFrame> SendAndReceiveAsync(MoCoBusFrame frame);
        Task<MoCoBusFrame> ReceiveAsync();
    }
}
