using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MDKControl.Core.Contracts;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Core.Services
{
    public delegate IMdkDeviceService MdkBleDeviceServiceFactory(Ble.IDevice device);

    public class MdkBleDeviceService : IMdkDeviceService
    {
        private readonly Ble.IAdapter _adapter;
        private Ble.IDevice _device;
        private Ble.IService _moCoBusService;
        private Ble.ICharacteristic _moCoBusRxCharacteristic;
        private Ble.ICharacteristic _moCoBusTxCharacteristic;
        private readonly ConcurrentQueue<byte> _rxBytesQueue = new ConcurrentQueue<byte>();
        private readonly AutoResetEvent _newRxBytesReceived = new AutoResetEvent(false);

        public MdkBleDeviceService(Ble.IDevice device, Ble.IAdapter adapter)
        {
            _device = device;
            _adapter = adapter;

            _adapter.DeviceConnected += (s, e) =>
            {
                if (e.Device.ID == _device.ID)
                {
                    _device = e.Device;
                    _device.ServicesDiscovered += (ss, ee) =>
                    {
                        _moCoBusService = _device.Services.SingleOrDefault(svc => svc.ID == MdkBleConstants.ServiceMoCoBus);
                        if (_moCoBusService != null)
                        {
                            _moCoBusRxCharacteristic =
                                _moCoBusService.Characteristics.SingleOrDefault(
                                    ch => ch.ID == MdkBleConstants.ServiceMoCoBusCharacteristicRx);
                            _moCoBusTxCharacteristic =
                                    _moCoBusService.Characteristics.SingleOrDefault(
                                        ch => ch.ID == MdkBleConstants.ServiceMoCoBusCharacteristicTx);

                            if (_moCoBusRxCharacteristic != null)
                            {
                                _moCoBusRxCharacteristic.ValueUpdated += (sss, eee) =>
                                {
                                    foreach (var b in eee.Characteristic.Value)
                                    {
                                        _rxBytesQueue.Enqueue(b);
                                    }
                                    _newRxBytesReceived.Set();
                                };
                                _moCoBusRxCharacteristic.StartUpdates();
                            }

                            IsConnected = true;
                        }
                    };
                        
                    //IsConnected = true;

                    _device.DiscoverServices();
                }
            };
            _adapter.DeviceDisconnected += (s, e) =>
            {
                if (e.Device.ID == _device.ID)
                {
                    IsConnected = false;
                }
            };
        }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            _adapter.ConnectToDevice(_device);
        }

        public void Disconnect()
        {
            _adapter.DisconnectDevice(_device);
        }

        public void SendBytes(byte[] data)
        {
            if (!IsConnected || _moCoBusTxCharacteristic == null)
                return;

            _moCoBusTxCharacteristic.Write(data);
        }

        public async Task<byte[]> SendAndReceiveBytesAsync(byte[] data)
        {
            SendBytes(data);
            return await ReceiveBytesAsync().ConfigureAwait(false);
        }

        public async Task<byte[]> ReceiveBytesAsync()
        {
            if (!IsConnected || _moCoBusRxCharacteristic == null)
                return null;

            return await Task.Factory.StartNew(() =>
            {
                _newRxBytesReceived.WaitOne();
                return _rxBytesQueue.ToArray();
            }).ConfigureAwait(false);
        }
    }
}
