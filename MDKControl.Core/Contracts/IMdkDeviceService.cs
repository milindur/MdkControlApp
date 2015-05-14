using System.Threading.Tasks;

namespace MDKControl.Core.Contracts
{
    public interface IMdkDeviceService
    {
        void Connect();
        void Disconnect();
        bool IsConnected { get; }

        void SendBytes(byte[] data);
        Task<byte[]> SendAndReceiveBytesAsync(byte[] data);
        Task<byte[]> ReceiveBytesAsync();
    }
}
