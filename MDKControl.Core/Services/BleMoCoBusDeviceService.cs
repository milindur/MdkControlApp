using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MDKControl.Core.Models;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Core.Services
{
    public delegate IMoCoBusDeviceService BleMoCoBusDeviceServiceFactory(Ble.IDevice device);

    public class BleMoCoBusDeviceService : MoCoBusDeviceServiceBase
    {
        private readonly Ble.IAdapter _adapter;
        private readonly AutoResetEvent _newRxBytesReceived = new AutoResetEvent(false);
        private readonly ConcurrentQueue<byte> _rxBytesQueue = new ConcurrentQueue<byte>();
        private Ble.IDevice _device;
        private Ble.ICharacteristic _moCoBusRxCharacteristic;
        private Ble.IService _moCoBusService;
        private Ble.ICharacteristic _moCoBusTxCharacteristic;

        public BleMoCoBusDeviceService(Ble.IDevice device, Ble.IAdapter adapter)
        {
            _device = device;
            _adapter = adapter;

            _adapter.DeviceConnected += AdapterOnDeviceConnected;
            _adapter.DeviceDisconnected += AdapterOnDeviceDisconnected;
        }

        private void AdapterOnDeviceConnected(object sender, Ble.DeviceConnectionEventArgs e)
        {
            if (e.Device.ID != _device.ID)
                return;

            _device = e.Device;
            _device.ServicesDiscovered += DeviceOnServicesDiscovered;
            _device.DiscoverServices();
        }

        private void DeviceOnServicesDiscovered(object sender, EventArgs e)
        {
            _moCoBusService = _device.Services
                .SingleOrDefault(svc => svc.ID == BleConstants.ServiceMoCoBus);
            if (_moCoBusService == null)
                return;

            _moCoBusService.CharacteristicsDiscovered += MoCoBusServiceOnCharacteristicsDiscovered;
            _moCoBusService.DiscoverCharacteristics();
        }

        private void AdapterOnDeviceDisconnected(object sender, Ble.DeviceConnectionEventArgs e)
        {
            if (e.Device.ID == _device.ID)
            {
                ConnectionState = ConnectionState.Disconnected;
            }
        }

        private void MoCoBusServiceOnCharacteristicsDiscovered(object sender, EventArgs e)
        {
            _moCoBusRxCharacteristic = _moCoBusService.Characteristics
                .SingleOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicRx);
            _moCoBusTxCharacteristic = _moCoBusService.Characteristics
                .SingleOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicTx);

            if (_moCoBusRxCharacteristic != null && _moCoBusRxCharacteristic.CanUpdate)
            {
                _moCoBusRxCharacteristic.ValueUpdated += MoCoBusRxCharacteristicOnValueUpdated;
                _moCoBusRxCharacteristic.StartUpdates();
            }

            ConnectionState = ConnectionState.Connected;
        }

        private void MoCoBusRxCharacteristicOnValueUpdated(object sender, Ble.CharacteristicReadEventArgs e)
        {
            Debug.Assert(e.Characteristic.Value != null, "e.Characteristic.Value != null");
            
            foreach (var b in e.Characteristic.Value)
            {
                _rxBytesQueue.Enqueue(b);
            }
            Debug.WriteLine(string.Join(":", e.Characteristic.Value.Select(x => x.ToString("X2")).ToArray()));
            _newRxBytesReceived.Set();
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
                    if (MoCoBusFrame.TryParse(_rxBytesQueue.ToArray(), out frame))
                    {
                        return frame;
                    }
                    return (MoCoBusFrame)null;
                }).ConfigureAwait(false);
        }
    }
}