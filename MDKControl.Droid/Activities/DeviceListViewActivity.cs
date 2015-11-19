using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Droid.Activities
{
    [Activity(Label = "Device Selection", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DeviceListViewActivity : ActivityBaseEx, AdapterView.IOnItemClickListener
    {
        private ListView _devicesList;

        public DeviceListViewActivity()
        {
        }

        public DeviceListViewModel Vm
        {
            get { return ServiceLocator.Current.GetInstance<DeviceListViewModel>(); }
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Vm.SelectDeviceCommand.Execute(Vm.Devices[position]);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DeviceListView);

            App.Initialize(this);

            DevicesList.Adapter = Vm.Devices.GetAdapter(GetDevicesAdapter);
            DevicesList.OnItemClickListener = this;
        }

        protected override void OnResume()
        {
            base.OnResume();

            App.Initialize(this);
            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
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
                        menuScanningStart.SetVisible(!Vm.IsScanning);
                        menuScanningStop.SetVisible(Vm.IsScanning);
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

        public ListView DevicesList
        {
            get
            {
                return _devicesList
                    ?? (_devicesList = FindViewById<ListView>(Resource.Id.DevicesList));
            }
        }
    }
}
