using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Helpers;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Core.ViewModels
{
    public class DeviceListViewModel : ViewModelBase
    {
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly INavigationService _navigationService;
        private readonly IAdapter _adapter;
        private readonly Func<IDevice, DeviceViewModel> _deviceViewModelFactory;
        private ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();
        private readonly Dictionary<Guid, DeviceViewModel> _deviceViewModels = new Dictionary<Guid, DeviceViewModel>();
        private RelayCommand _startScanCommand;
        private RelayCommand _stopScanCommand;
        private RelayCommand<IDevice> _selectDeviceCommand;
        private bool _isScanning;

        public DeviceListViewModel(IDispatcherHelper dispatcherHelper, 
                                   INavigationService navigationService, 
                                   IAdapter adapter, 
                                   Func<IDevice, DeviceViewModel> deviceViewModelFactory)
        {
            _dispatcherHelper = dispatcherHelper;
            _navigationService = navigationService;
            _adapter = adapter;
            _deviceViewModelFactory = deviceViewModelFactory;

            _adapter.ScanTimeoutElapsed += (s, e) =>
            {
                StopScan();
            };
            _adapter.DeviceDiscovered += (s, e) =>
            {
                if (_devices.All(d => d.ID != e.Device.ID))
                {
                    _dispatcherHelper.RunOnUIThread(() =>
                        {
                            _devices.Add(e.Device);
                        });
                }
            };
        }

        public bool IsScanning
        {
            get { return _isScanning; }
            private set
            {
                _isScanning = value;
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        StartScanCommand.RaiseCanExecuteChanged();
                        StopScanCommand.RaiseCanExecuteChanged();
                        RaisePropertyChanged(() => IsScanning);
                    });
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
                _dispatcherHelper.RunOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => Devices);
                    });
            }
        }

        public RelayCommand StartScanCommand
        {
            get { return _startScanCommand ?? (_startScanCommand = new RelayCommand(StartScan, () => !IsScanning)); }
        }

        public RelayCommand StopScanCommand
        {
            get { return _stopScanCommand ?? (_stopScanCommand = new RelayCommand(StopScan, () => IsScanning)); }
        }

        public RelayCommand<IDevice> SelectDeviceCommand
        {
            get { return _selectDeviceCommand ?? (_selectDeviceCommand = new RelayCommand<IDevice>(SelectDevice)); }
        }

        private void StartScan()
        {
            Devices.Clear();

            foreach (var vm in _deviceViewModels.Values)
            {
                vm.Cleanup();
            }
            _deviceViewModels.Clear();

            IsScanning = true;
            _adapter.StartScanningForDevices(BleConstants.ServiceMoCoBus);
        }

        private void StopScan()
        {
            IsScanning = false;
            _adapter.StopScanningForDevices();
        }

        private void SelectDevice(IDevice device)
        {
            StopScan();

            DeviceViewModel vm;
            if (!_deviceViewModels.TryGetValue(device.ID, out vm))
            {
                vm = _deviceViewModelFactory(device);
                _deviceViewModels.Add(device.ID, vm);
            }
            _navigationService.NavigateTo(ViewModelLocator.DeviceViewKey, vm);
        }

        public override void Cleanup()
        {
            Devices.Clear();

            foreach (var vm in _deviceViewModels.Values)
            {
                vm.Cleanup();
            }
            _deviceViewModels.Clear();

            base.Cleanup();
        }
    }
}
