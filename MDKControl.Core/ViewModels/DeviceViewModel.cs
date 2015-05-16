using System;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IMoCoBusDeviceService _deviceService;

        public DeviceViewModel(IMoCoBusDeviceService deviceService)
        {
            _deviceService = deviceService;
            _deviceService.ConnectionChanged += DeviceServiceOnConnectionChanged;
        }

        private void DeviceServiceOnConnectionChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => IsConnected);
        }

        public bool IsConnected
        {
            get
            {
                return _deviceService.ConnectionState == ConnectionState.Connecting
                       || _deviceService.ConnectionState == ConnectionState.Connected;
            }
            set
            {
                if (_deviceService.ConnectionState == ConnectionState.Disconnected && value)
                {
                    _deviceService.Connect();
                }
                else if (_deviceService.ConnectionState != ConnectionState.Disconnected && !value)
                {
                    _deviceService.Disconnect();
                }
                OnPropertyChanged(() => IsConnected);
            }
        }
    }
}
