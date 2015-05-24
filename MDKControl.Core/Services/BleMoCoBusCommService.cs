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
    public class BleMoCoBusCommService : MoCoBusCommServiceBase
    {
        private readonly Ble.IAdapter _adapter;
        private readonly AutoResetEvent _newRxBytesReceived = new AutoResetEvent(false);
        private ConcurrentQueue<byte[]> _rxBytesQueue = new ConcurrentQueue<byte[]>();
        private Ble.IDevice _device;
        private Ble.IService _moCoBusService;
        private Ble.ICharacteristic _moCoBusRxCharacteristic;
        private Ble.ICharacteristic _moCoBusTxCharacteristic;

        public BleMoCoBusCommService(Ble.IDevice device, Ble.IAdapter adapter)
        {
            _device = device;
            _adapter = adapter;

            _adapter.DeviceConnected += AdapterOnDeviceConnected;
            _adapter.DeviceDisconnected += AdapterOnDeviceDisconnected;
        }

        private void AdapterOnDeviceConnected(object sender, Ble.DeviceConnectionEventArgs e)
        {
            if (e.Device.ID != _device.ID) return;

            _device = e.Device;
            _device.ServicesDiscovered += DeviceOnServicesDiscovered;
            _device.DiscoverServices();
        }

        private void AdapterOnDeviceDisconnected(object sender, Ble.DeviceConnectionEventArgs e)
        {
            if (e.Device.ID != _device.ID) return;

            _device = e.Device;
            ConnectionState = ConnectionState.Disconnected;
        }

        private void DeviceOnServicesDiscovered(object sender, EventArgs e)
        {
            _moCoBusService = _device.Services
                .FirstOrDefault(svc => svc.ID == BleConstants.ServiceMoCoBus);
            if (_moCoBusService == null)
                return;

            _moCoBusService.CharacteristicsDiscovered += MoCoBusServiceOnCharacteristicsDiscovered;
            _moCoBusService.DiscoverCharacteristics();
        }

        private void MoCoBusServiceOnCharacteristicsDiscovered(object sender, EventArgs e)
        {
            _moCoBusRxCharacteristic = _moCoBusService.Characteristics
                .FirstOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicRx);
            _moCoBusTxCharacteristic = _moCoBusService.Characteristics
                .FirstOrDefault(ch => ch.ID == BleConstants.ServiceMoCoBusCharacteristicTx);

            if (_moCoBusRxCharacteristic != null)
            {
                _moCoBusRxCharacteristic.ValueUpdated += MoCoBusRxCharacteristicOnValueUpdated;
                _moCoBusRxCharacteristic.StartUpdates();
            }

            ConnectionState = ConnectionState.Connected;
        }

        private void MoCoBusRxCharacteristicOnValueUpdated(object sender, Ble.CharacteristicReadEventArgs e)
        {
            Debug.Assert(e.Characteristic.Value != null, "e.Characteristic.Value != null");
            Debug.WriteLine(string.Join(":", e.Characteristic.Value.Select(x => x.ToString("X2")).ToArray()));

            _rxBytesQueue.Enqueue(e.Characteristic.Value);
            _newRxBytesReceived.Set();
            OnDataReceived();
        }

        public override void Connect()
        {
            _adapter.ConnectToDevice(_device);
            ConnectionState = ConnectionState.Connecting;
        }

        public override void Disconnect()
        {
            _moCoBusTxCharacteristic = null;

            if (_moCoBusRxCharacteristic != null)
            {
                _moCoBusRxCharacteristic.ValueUpdated -= MoCoBusRxCharacteristicOnValueUpdated;
                _moCoBusRxCharacteristic.StopUpdates();
                _moCoBusRxCharacteristic = null;
            }

            if (_moCoBusService != null)
            {
                _moCoBusService.CharacteristicsDiscovered -= MoCoBusServiceOnCharacteristicsDiscovered;
                _moCoBusService = null;
            }

            _device.ServicesDiscovered -= DeviceOnServicesDiscovered;
            _adapter.DisconnectDevice(_device);
        }

        public override void Send(MoCoBusFrame frame)
        {
            if (!IsConnected || _moCoBusTxCharacteristic == null)
                return;

            _moCoBusTxCharacteristic.Write(frame.ToByteArray());
        }

        public override void ClearReceiveBuffer()
        {
            _rxBytesQueue = new ConcurrentQueue<byte[]>();
        }

        public override async Task<MoCoBusFrame> ReceiveAsync()
        {
            if (!IsConnected || _moCoBusRxCharacteristic == null)
                return null;

            return await Task.Factory.StartNew(() =>
                {
                    _newRxBytesReceived.WaitOne();
                    byte[] bytes;
                    if (_rxBytesQueue.TryDequeue(out bytes)) {
                        MoCoBusFrame frame;
                        if (MoCoBusFrame.TryParse(bytes, out frame))
                        {
                            return frame;
                        }
                    }
                    return (MoCoBusFrame)null;
                }).ConfigureAwait(false);
        }
    }
}