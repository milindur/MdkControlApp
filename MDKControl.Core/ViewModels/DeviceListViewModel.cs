using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace MDKControl.Core.ViewModels
{
    public class DeviceListViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAdapter _adapter;
        private readonly Func<IDevice, DeviceViewModel> _deviceViewModelFactory;
        private ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();
        private RelayCommand _startScanCommand;
        private RelayCommand _stopScanCommand;
        private RelayCommand<IDevice> _selectDeviceCommand;
        private bool _isScanning;

        public DeviceListViewModel(INavigationService navigationService, 
            IAdapter adapter, 
            Func<IDevice, DeviceViewModel> deviceViewModelFactory)
        {
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
                    _devices.Add(e.Device);
                }
            };
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
                ((RelayCommand)StartScanCommand).RaiseCanExecuteChanged();
                ((RelayCommand)StopScanCommand).RaiseCanExecuteChanged();
                RaisePropertyChanged(() => IsScanning);
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
                RaisePropertyChanged();
            }
        }

        public RelayCommand StartScanCommand
        {
            get { return _startScanCommand ?? (_startScanCommand = new RelayCommand(StartScan, () => !IsScanning)); }
        }

        private void StartScan()
        {
            IsScanning = true;
            _adapter.StartScanningForDevices(BleConstants.ServiceMoCoBus);
        }

        public RelayCommand StopScanCommand
        {
            get { return _stopScanCommand ?? (_stopScanCommand = new RelayCommand(StopScan, () => IsScanning)); }
        }

        private void StopScan()
        {
            IsScanning = false;
            _adapter.StopScanningForDevices();
        }

        public RelayCommand<IDevice> SelectDeviceCommand
        {
            get { return _selectDeviceCommand ?? (_selectDeviceCommand = new RelayCommand<IDevice>(SelectDevice)); }
        }

        private void SelectDevice(IDevice device)
        {
            StopScan();
            _navigationService.NavigateTo(ViewModelLocator.DeviceViewKey, _deviceViewModelFactory(device));
        }
    }
}
