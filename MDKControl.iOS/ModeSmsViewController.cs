using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;

namespace MDKControl.iOS
{
	partial class ModeSmsViewController : UITableViewController, INavigationTarget
	{
		public ModeSmsViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }

        public ModeSmsViewModel Vm { get; private set; }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewController ViewDidLoad");

            Vm = (ModeSmsViewModel)NavigationParameter;

            var tableView = (UITableView)View;
            tableView.BackgroundColor = Colors.DefaultLightGray;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;
            
            /*StartTableViewCell.SetCommand(
                "Clicked",
                );*/
            

            base.ViewDidLoad();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            System.Diagnostics.Debug.WriteLine(tableView.CellAt(indexPath));

            //base.RowSelected(tableView, indexPath);
        }
	}
}
