using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using Microsoft.Practices.ServiceLocation;
using Robotics.Mobile.Core.Bluetooth.LE;
using MDKControl.Core.ViewModels;

namespace MDKControl.iOS
{
	partial class DeviceListViewController : UIViewController
	{
        private ObservableTableViewController<IDevice> _tableViewController;
        
		public DeviceListViewController (IntPtr handle) : base (handle)
		{
		}

        public DeviceListViewModel Vm
        {
            get { return ServiceLocator.Current.GetInstance<DeviceListViewModel>(); }
        }

        public override void ViewDidLoad()
        {
            Vm.PropertyChanged += (s, e) => {};
            ScanButton.Clicked += (s, e) => {};

            ScanButton.SetCommand(
                "Clicked",
                Vm.StartScanCommand);

            _tableViewController = Vm.Devices.GetController(CreateDeviceCell, BindDeviceCell);
            _tableViewController.TableView = DevicesTableView;
            _tableViewController.SelectionChanged += (s, e) => Vm.SelectDeviceCommand.Execute(_tableViewController.SelectedItem);

            base.ViewDidLoad();
        }

        private void BindDeviceCell(UITableViewCell cell, IDevice device, NSIndexPath path)
        {
            cell.TextLabel.Text = device.Name;
        }

        private UITableViewCell CreateDeviceCell(NSString reuseId)
        {
            var cell = new UITableViewCell(UITableViewCellStyle.Default, null);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            return cell;
        }
    }
}
