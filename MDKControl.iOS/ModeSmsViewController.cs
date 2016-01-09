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
        private Binding _exposureTimeBinding;
        private Binding _delayTimeBinding;
        private Binding _intervalTimeBinding;
        private Binding _durationTimeBinding;
        private Binding _maxShotsBinding;

		public ModeSmsViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }

        public ModeSmsViewModel Vm { get; private set; }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewController ViewDidLoad");

            Vm = (ModeSmsViewModel)NavigationParameter;

            Vm.PropertyChanged += (s, e) => {};

            var tableView = (UITableView)View;
            tableView.BackgroundColor = Colors.DefaultLightGray;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            PreDelayValueLabel.Text = string.Format("{0:F1}s", 0.1f); 
            
            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime)
                .WhenSourceChanges(() =>
                    { 
                        ExposureValueLabel.Text = string.Format("{0:F1}s", Vm.ExposureTime);
                    });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime)
                .WhenSourceChanges(() =>
                    { 
                        PostDelayValueLabel.Text = string.Format("{0:F1}s", Vm.DelayTime); 
                    });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _intervalTimeBinding = this.SetBinding(() => Vm.IntervalTime)
                .WhenSourceChanges(() =>
                    {
                        IntervalValueLabel.Text = string.Format("{0:F1}s", Vm.IntervalTime);
                    });
            _intervalTimeBinding.ForceUpdateValueFromSourceToTarget();

            _durationTimeBinding = this.SetBinding(() => Vm.DurationTime)
                .WhenSourceChanges(() =>
                    {
                        DurationValueLabel.Text = string.Format("{0}:{1:00}m", (int)(Vm.DurationTime / 60), (int)Vm.DurationTime % 60);
                    });
            _durationTimeBinding.ForceUpdateValueFromSourceToTarget();

            _maxShotsBinding = this.SetBinding(() => Vm.MaxShots)
                .WhenSourceChanges(() =>
                    {
                        ShotsValueLabel.Text = string.Format("{0}", Vm.MaxShots);
                    });
            _maxShotsBinding.ForceUpdateValueFromSourceToTarget();

            base.ViewDidLoad();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            System.Diagnostics.Debug.WriteLine(tableView.CellAt(indexPath));
        }
	}
}
