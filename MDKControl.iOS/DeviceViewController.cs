using System;
using CoreGraphics;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Extensions;
using Microsoft.Practices.ServiceLocation;
using UIKit;

namespace MDKControl.iOS
{
    partial class DeviceViewController : UITableViewController, INavigationTarget
	{
        private bool _modeChangeRequested = false;
        
        private Binding _isConnectedBinding;
        private Binding _programModeBinding;

		public DeviceViewController (IntPtr handle) : base (handle)
		{
		}

        private UISwitch ConnectButton { get; } = new UISwitch();

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
            tableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            tableView.BackgroundView = null;
            tableView.AllowsSelection = true;

            ModeSmsTableViewCell.ImageView.Image = new UIImage("Timer.png").MakeThumb(new CGSize(29, 29));
            ModePanoramaTableViewCell.ImageView.Image = new UIImage("PanoramaFilled.png").MakeThumb(new CGSize(29, 29));
            ModeAstroTableViewCell.ImageView.Image = new UIImage("TelescopeFilled.png").MakeThumb(new CGSize(29, 29));

            NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(ConnectButton), false);

            SetupBindings();

            base.ViewDidLoad();
        }

        public override void ViewDidAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("DeviceViewController ViewDidAppear");

            SetupBindings();

            base.ViewDidAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("DeviceViewController ViewDidDisappear");

            DetachBindings();

            base.ViewDidDisappear(animated);
        }

        void SetupBindings()
        {
            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectButton.On, BindingMode.TwoWay).UpdateTargetTrigger("ValueChanged");
            _isConnectedBinding.ValueChanged += (sender, e) => 
            {
                System.Diagnostics.Debug.WriteLine("DeviceViewModel PropertyChanged IsConnected");
                OnConnectionOrProgramModeChanged();
            };
            _isConnectedBinding.ForceUpdateValueFromSourceToTarget();

            _programModeBinding = this.SetBinding(() => Vm.ProgramMode).WhenSourceChanges(() => 
            {
                System.Diagnostics.Debug.WriteLine("DeviceViewModel PropertyChanged ProgramMode");
                OnConnectionOrProgramModeChanged();
                var navService = ServiceLocator.Current.GetInstance<INavigationService>();
                switch (Vm.ProgramMode)
                {
                    case MoCoBusProgramMode.ShootMoveShoot:
                        if (navService.CurrentPageKey != AppDelegate.ModeSmsViewKey && navService.CurrentPageKey != AppDelegate.ModeSmsStatusViewKey)
                        {
                            if (_modeChangeRequested || Vm.RunStatus != MoCoBusRunStatus.Stopped)
                            {
                                System.Diagnostics.Debug.WriteLine("Navigating to ModeSmsView since a mode change was requested");
                                _modeChangeRequested = false;
                                navService.NavigateTo(AppDelegate.ModeSmsViewKey, Vm.ModeSmsViewModel);
                            }
                        }
                        break;
                    case MoCoBusProgramMode.Panorama:
                        if (navService.CurrentPageKey != AppDelegate.ModePanoViewKey && navService.CurrentPageKey != AppDelegate.ModePanoStatusViewKey)
                        {
                            if (_modeChangeRequested || Vm.RunStatus != MoCoBusRunStatus.Stopped)
                            {
                                System.Diagnostics.Debug.WriteLine("Navigating to ModePanoView since a mode change was requested");
                                _modeChangeRequested = false;
                                navService.NavigateTo(AppDelegate.ModePanoViewKey, Vm.ModePanoViewModel);
                            }
                        }
                        break;
                    case MoCoBusProgramMode.Astro:
                        if (navService.CurrentPageKey != AppDelegate.ModeAstroViewKey && navService.CurrentPageKey != AppDelegate.ModeAstroStatusViewKey)
                        {
                            if (_modeChangeRequested || Vm.RunStatus != MoCoBusRunStatus.Stopped)
                            {
                                System.Diagnostics.Debug.WriteLine("Navigating to ModeAstroView since a mode change was requested");
                                _modeChangeRequested = false;
                                navService.NavigateTo(AppDelegate.ModeAstroViewKey, Vm.ModeAstroViewModel);
                            }
                        }
                        break;
                }
            });
            _programModeBinding.ForceUpdateValueFromSourceToTarget();
        }

        private void DetachBindings()
        {
            _isConnectedBinding?.Detach();
            _isConnectedBinding = null;

            _programModeBinding?.Detach();
            _programModeBinding = null;
        }

        private void OnConnectionOrProgramModeChanged()
        {
            if (!Vm.IsConnected && View.UserInteractionEnabled)
            {
                UIView.Animate(0.5, () => 
                    {
                        View.UserInteractionEnabled = false;
                        View.Alpha = 0.5f;
                    });                
            }

            if (Vm.IsConnected && Vm.ProgramMode != MoCoBusProgramMode.Invalid && !View.UserInteractionEnabled)
            {
                UIView.Animate(0.5, () => 
                    {
                        View.UserInteractionEnabled = true;
                        View.Alpha = 1.0f;
                    });                
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            System.Diagnostics.Debug.WriteLine("DeviceViewController RowSelected");
            System.Diagnostics.Debug.WriteLine(tableView.CellAt(indexPath));

            switch (indexPath.Section)
            {
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 0: // sms
                                _modeChangeRequested = true;
                                Vm.SetModeSmsCommand.Execute(null);
                                break;
                            case 1: // pano
                                _modeChangeRequested = true;
                                Vm.SetModePanoCommand.Execute(null);
                                break;
                            case 2: // astro
                                _modeChangeRequested = true;
                                Vm.SetModeAstroCommand.Execute(null);
                                break;
                        }
                    }
                    break;
            }

            tableView.DeselectRow(indexPath, true);
        }
	}
}
