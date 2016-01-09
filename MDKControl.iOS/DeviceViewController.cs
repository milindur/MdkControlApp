using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Extensions;
using CoreGraphics;

namespace MDKControl.iOS
{
    partial class DeviceViewController : UITableViewController, INavigationTarget
	{
        private Binding _isConnectedBinding;
        private Binding _modeBinding;

        UISwitch ConnectButton { get; } = new UISwitch();

		public DeviceViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }

        public DeviceViewModel Vm { get; private set; }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("DeviceViewController ViewDidLoad");

            Vm = (DeviceViewModel)NavigationParameter;

            Vm.PropertyChanged += (s, e) => {};
            ConnectButton.ValueChanged += (s, e) => {};
            ConnectButton.On = false;

            var tableView = (UITableView)View;
            tableView.BackgroundColor = Colors.DefaultLightGray;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            ModeSmsTableViewCell.ImageView.Image = new UIImage("Timer.png").MakeThumb(new CGSize(29, 29));
            ModePanoramaTableViewCell.ImageView.Image = new UIImage("PanoramaFilled.png").MakeThumb(new CGSize(29, 29));
            ModeAstroTableViewCell.ImageView.Image = new UIImage("TelescopeFilled.png").MakeThumb(new CGSize(29, 29));

            //ConnectButton = new UISwitch();
            NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(ConnectButton), false);

            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectButton.On, BindingMode.TwoWay)
                .UpdateTargetTrigger("ValueChanged");
            _isConnectedBinding.ValueChanged += (sender, e) => 
                {
                    UIView.Animate(0.5, () => 
                        {
                            if (Vm.IsConnected)
                            {
                                View.UserInteractionEnabled = true;
                                View.Alpha = 1.0f;
                            }
                            else
                            {
                                View.UserInteractionEnabled = false;
                                View.Alpha = 0.5f;

                            }
                        });
                };
            _isConnectedBinding.ForceUpdateValueFromSourceToTarget();

            /*_modeBinding = this.SetBinding(() => ModeButton.SelectedSegment, () => Vm.ProgramMode, BindingMode.TwoWay)
                .ConvertTargetToSource(m =>
                    {
                        switch (m)
                        {
                            default:
                            case MoCoBusProgramMode.ShootMoveShoot:
                                return 0;
                            case MoCoBusProgramMode.Panorama:
                                return 1;
                            case MoCoBusProgramMode.Astro:
                                return 2;
                        }
                    })
                .WhenSourceChanges(() =>
                    {
                        if (ModeButton.SelectedSegment == 0 && Vm.ProgramMode != MoCoBusProgramMode.ShootMoveShoot)
                        {
                            Vm.SetModeSmsCommand.Execute(null);
                        }
                        else if (ModeButton.SelectedSegment == 1 && Vm.ProgramMode != MoCoBusProgramMode.Panorama)
                        {
                            Vm.SetModePanoCommand.Execute(null);
                        }
                        else if (ModeButton.SelectedSegment == 2 && Vm.ProgramMode != MoCoBusProgramMode.Astro)
                        {
                            Vm.SetModeAstroCommand.Execute(null);
                        }
                    })
                .UpdateSourceTrigger("ValueChanged");*/
                

            /*Vm.PropertyChanged += (sender, e) => 
                {
                    if (e.PropertyName == "ProgramMode")
                    {
                        switch (Vm.ProgramMode)
                        {
                            case MoCoBusProgramMode.ShootMoveShoot:
                                if (ModeButton.SelectedSegment != 0) ModeButton.SelectedSegment = 0;
                                break;
                            case MoCoBusProgramMode.Panorama:
                                if (ModeButton.SelectedSegment != 1) ModeButton.SelectedSegment = 1;
                                break;
                            case MoCoBusProgramMode.Astro:
                                if (ModeButton.SelectedSegment != 2) ModeButton.SelectedSegment = 2;
                                break;
                        }
                    }
                };

            ModeButton.ValueChanged += (sender, e) => 
                {
                    if (ModeButton.SelectedSegment == 0 && Vm.ProgramMode != MoCoBusProgramMode.ShootMoveShoot)
                    {
                        Vm.SetModeSmsCommand.Execute(null);
                    }
                    else if (ModeButton.SelectedSegment == 1 && Vm.ProgramMode != MoCoBusProgramMode.Panorama)
                    {
                        Vm.SetModePanoCommand.Execute(null);
                    }
                    else if (ModeButton.SelectedSegment == 2 && Vm.ProgramMode != MoCoBusProgramMode.Astro)
                    {
                        Vm.SetModeAstroCommand.Execute(null);
                    }
                };*/

            base.ViewDidLoad();
        }

        public override void ViewWillUnload()
        {
            
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);
            System.Diagnostics.Debug.WriteLine(cell);

            //cell.SetSelected(false, true);

            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(AppDelegate.ModeSmsViewKey, Vm.ModeSmsViewModel);
        }
	}
}
