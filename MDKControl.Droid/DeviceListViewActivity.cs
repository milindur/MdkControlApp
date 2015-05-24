using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher")]
    public class DeviceListViewActivity : ActivityBaseEx, AdapterView.IOnItemClickListener
    {
        private Binding _isScanningBinding;
        private ListView _devicesList;
        private Button _refreshButton;

        public DeviceListViewActivity()
        {
        }

        public DeviceListViewModel Vm
        {
            get { return ServiceLocator.Current.GetInstance<DeviceListViewModel>(); }
        }

        public ListView DevicesList
        {
            get
            {
                return _devicesList
                    ?? (_devicesList = FindViewById<ListView>(Resource.Id.DevicesList));
            }
        }

        public Button RefreshButton
        {
            get
            {
                return _refreshButton
                    ?? (_refreshButton = FindViewById<Button>(Resource.Id.RefreshButton));
            }
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Vm.SelectDeviceCommand.Execute(Vm.Devices[position]);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DeviceListView);

            //Vm = GlobalNavigation.GetAndRemoveParameter<DeviceListViewModel>(Intent);

            // Avoid aggressive linker problem which removes the Click event
            RefreshButton.Click += (s, e) => {};
            RefreshButton.SetCommand("Click", Vm.StartScanCommand);

            DevicesList.Adapter = Vm.Devices.GetAdapter(GetDevicesAdapter);
            DevicesList.OnItemClickListener = this;
        }

        protected override void OnResume()
        {
            base.OnResume();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DeviceListOptionsMenu, menu);

            var menuScanningStart = menu.FindItem(Resource.Id.ScanningStart);
            var menuScanningStop = menu.FindItem(Resource.Id.ScanningStop);

            menuScanningStart.SetVisible(!Vm.IsScanning);
            menuScanningStop.SetVisible(Vm.IsScanning);

            Vm.PropertyChanged += (o, e) => 
                {
                    if (e.PropertyName == "IsScanning")
                    {
                        RunOnUiThread(() =>
                            {
                                menuScanningStart.SetVisible(!Vm.IsScanning);
                                menuScanningStop.SetVisible(Vm.IsScanning);
                            });
                    }
                };

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.ScanningStart:
                    Vm.StartScanCommand.Execute(null);
                    return true;
                case Resource.Id.ScanningStop:
                    Vm.StopScanCommand.Execute(null);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private View GetDevicesAdapter(int position, Ble.IDevice device, View convertView)
        {
            convertView = LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            var title = convertView.FindViewById<TextView>(Android.Resource.Id.Text1);
            title.Text = device.Name;

            return convertView;
        }
    }
}
