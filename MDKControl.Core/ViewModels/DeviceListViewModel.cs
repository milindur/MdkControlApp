using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.ViewModels
{
    public class DeviceListViewModel : ViewModelBase
    {
        IDevice _selectedDevice;
        ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();

        public DeviceListViewModel()
        {
            MessagingCenter.Subscribe<DeviceViewModel, IDevice>(this, "TodoSaved", (sender, model) =>
                {
                });

            MessagingCenter.Subscribe<DeviceViewModel, IDevice>(this, "TodoDeleted", (sender, model) =>
                {
                });
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
                    var todovm = new DeviceViewModel();
                    //Navigation.Push(ViewFactory.CreatePage(todovm));
                    _selectedDevice = null;
                }
            }
        }
    }
}
