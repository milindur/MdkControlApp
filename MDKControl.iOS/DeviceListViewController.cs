using System;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Robotics.Mobile.Core.Bluetooth.LE;
using UIKit;

namespace MDKControl.iOS
{
    internal partial class DeviceListViewController : UIViewController
	{
        private ObservableTableViewController<IDevice> _tableViewController;
        
		public DeviceListViewController (IntPtr handle) : base (handle)
		{
		}

        public DeviceListViewModel Vm => ServiceLocator.Current.GetInstance<DeviceListViewModel>();

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

        private static void BindDeviceCell(UITableViewCell cell, IDevice device, NSIndexPath path)
        {
            cell.TextLabel.Text = device.Name;
        }

        private static UITableViewCell CreateDeviceCell(NSString reuseId)
        {
            var cell = new UITableViewCell(UITableViewCellStyle.Default, null)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            return cell;
        }
    }
}
