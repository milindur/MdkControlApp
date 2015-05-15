using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Core.Services
{
    public delegate IMoCoBusDeviceService BleMoCoBusDeviceServiceFactory(Ble.IDevice device);

    public class BleMoCoBusDeviceService : MoCoBusDeviceServiceBase, IMoCoBusDeviceService
    {
        private readonly Ble.IAdapter _adapter;
        private readonly AutoResetEvent _newRxBytesReceived = new AutoResetEvent(false);
        //private readonly ConcurrentQueue<byte> _rxBytesQueue = new ConcurrentQueue<byte>();
        private Ble.IDevice _device;
        private Ble.ICharacteristic _moCoBusRxCharacteristic;
        private Ble.IService _moCoBusService;
        private Ble.ICharacteristic _moCoBusTxCharacteristic;

        public BleMoCoBusDeviceService(Ble.IDevice device, Ble.IAdapter adapter)
        {
            _device = device;
            _adapter = adapter;

            _adapter.DeviceConnected += (s, e) =>
            {
                if (e.Device.ID != _device.ID) return;

                _device = e.Device;
                _device.ServicesDiscovered += (ss, ee) =>
                {
                    _moCoBusService = _device.Services
                        .SingleOrDefault(svc => svc.ID == BleConstants.ServiceMoCoBus);
                    if (_moCoBusService == null) return;

                    _moCoBusRxCharacteristic = _moCoBusService.Characteristics
                        .SingleOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicRx);
                    _moCoBusTxCharacteristic = _moCoBusService.Characteristics
                        .SingleOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicTx);

                    if (_moCoBusRxCharacteristic != null)
                    {
                        _moCoBusRxCharacteristic.ValueUpdated += (sss, eee) =>
                        {
                            foreach (var b in eee.Characteristic.Value)
                            {
                                //_rxBytesQueue.Enqueue(b);
                            }
                            _newRxBytesReceived.Set();
                        };
                        _moCoBusRxCharacteristic.StartUpdates();
                    }

                    ConnectionState = ConnectionState.Connected;
                };

                _device.DiscoverServices();
            };
            _adapter.DeviceDisconnected += (s, e) =>
            {
                if (e.Device.ID == _device.ID)
                {
                    ConnectionState = ConnectionState.Disconnected;
                }
            };
        }

        public override void Connect()
        {
            _adapter.ConnectToDevice(_device);
            ConnectionState = ConnectionState.Connecting;
        }

        public override void Disconnect()
        {
            _adapter.DisconnectDevice(_device);
        }

        public override void Send(MoCoBusFrame frame)
        {
            if (!IsConnected || _moCoBusTxCharacteristic == null)
                return;

            _moCoBusTxCharacteristic.Write(frame.ToByteArray());
        }

        public override async Task<MoCoBusFrame> ReceiveAsync()
        {
            if (!IsConnected || _moCoBusRxCharacteristic == null)
                return null;

            return await Task.Factory.StartNew(() =>
            {
                _newRxBytesReceived.WaitOne();
                MoCoBusFrame frame;
                /*if (MoCoBusFrame.TryParse(_rxBytesQueue.ToArray(), out frame))
                {
                    return frame;
                }*/
                return (MoCoBusFrame)null;
            }).ConfigureAwait(false);
        }
    }
}