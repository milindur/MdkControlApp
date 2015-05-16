using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;
using Xamarin.Forms;
using System.Diagnostics;

namespace MDKControl.Core.ViewModels
{
    public class DeviceListViewModel : ViewModelBase
    {
        private readonly IAdapter _adapter;
        private readonly BleMoCoBusDeviceServiceFactory _bleMoCoBusDeviceServiceFactory;
        private IDevice _selectedDevice;
        private ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();
        private Command _startScanCommand;
        private Command _stopScanCommand;
        private bool _isScanning;

        public DeviceListViewModel(IAdapter adapter, BleMoCoBusDeviceServiceFactory bleMoCoBusDeviceServiceFactory)
        {
            _adapter = adapter;
            _bleMoCoBusDeviceServiceFactory = bleMoCoBusDeviceServiceFactory;
            
            _adapter.ScanTimeoutElapsed += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                    {
                        StopScan();
                    });
            };
            _adapter.DeviceDiscovered += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                    {
                        if (_devices.All(d => d.ID != e.Device.ID))
                        {
                            _devices.Add(e.Device);
                        }
                    });
            };
            /*MessagingCenter.Subscribe<DeviceViewModel, IDevice>(this, "TodoSaved", (sender, model) =>
            {
            });

            MessagingCenter.Subscribe<DeviceViewModel, IDevice>(this, "TodoDeleted", (sender, model) =>
            {
            });*/

            Title = "Devices";
        }

        public bool IsScanning
        {
            get
            {
                return _isScanning;
            }
            private set
            {
                _isScanning = value;
                ((Command)StartScanCommand).ChangeCanExecute();
                ((Command)StopScanCommand).ChangeCanExecute();
                OnPropertyChanged(() => IsScanning);
            }
        }

        public ObservableCollection<IDevice> Devices
        { 
            get { return _devices; } 
            set
            {
                if (_devices == value)
                    return;
                _devices = value;
                OnPropertyChanged();
            }
        }

        public IDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (_selectedDevice == value)
                    return;
                
                _selectedDevice = value;

                OnPropertyChanged();

                if (_selectedDevice != null)
                {
                    var device = _bleMoCoBusDeviceServiceFactory(_selectedDevice);
                    device.ConnectionChanged += DeviceConnectionChanged;
                    device.Connect();
                }
            }
        }

        private async void DeviceConnectionChanged(object sender, System.EventArgs e)
        {
            var device = (IMoCoBusDeviceService)sender;
            
            if (device.IsConnected)
            {
                var res = await device.SendAndReceiveAsync(new Models.MoCoBusMainCommandFrame(3, Models.MoCoBusMainCommand.GetFirmwareVersion, new byte[0]));
                if (res != null) Debug.WriteLine(res);
            }
        }

        public object SelectedItem
        {
            get { return SelectedDevice; }
            set { SelectedDevice = (IDevice)value; }
        }

        public ICommand StartScanCommand
        {
            get { return _startScanCommand ?? (_startScanCommand = new Command(StartScan, () => !IsScanning)); }
        }

        private void StartScan()
        {
            IsScanning = true;
            _adapter.StartScanningForDevices(BleConstants.ServiceMoCoBus);
        }

        public ICommand StopScanCommand
        {
            get { return _stopScanCommand ?? (_stopScanCommand = new Command(StopScan, () => IsScanning)); }
        }

        private void StopScan()
        {
            IsScanning = false;
            _adapter.StopScanningForDevices();
        }
    }
}
