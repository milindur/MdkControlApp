using CoreGraphics;
using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Extensions;
using System.Threading.Tasks;

namespace MDKControl.iOS
{
    partial class DeviceViewController : UITableViewController, INavigationTarget
	{
        private bool _modeChangeRequested = false;
        
        private Binding _isConnectedBinding;
        private Binding _programModeBinding;
        private Binding _runStatusBinding;

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

            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectButton.On, BindingMode.TwoWay)
                .UpdateTargetTrigger("ValueChanged");
            _isConnectedBinding.ValueChanged += (sender, e) => 
                {
                    System.Diagnostics.Debug.WriteLine("DeviceViewModel PropertyChanged IsConnected");

                    OnConnectionOrProgramModeChanged();
                };
            _isConnectedBinding.ForceUpdateValueFromSourceToTarget();

            _programModeBinding = this.SetBinding(() => Vm.ProgramMode)
                .WhenSourceChanges(async () => 
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
                                    else if (navService.CurrentPageKey != ViewModelLocator.DeviceViewKey)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Navigating back to DeviceView since ProgramMode was changed to another mode");
                                        await NavigateBackToDeviceView();
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
                                        //navService.NavigateTo(AppDelegate.ModePanoViewKey, Vm.ModePanoViewModel);
                                    }
                                    else if (navService.CurrentPageKey != ViewModelLocator.DeviceViewKey)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Navigating back to DeviceView since ProgramMode was changed to another mode");
                                        await NavigateBackToDeviceView();
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
                                    else if (navService.CurrentPageKey != ViewModelLocator.DeviceViewKey)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Navigating back to DeviceView since ProgramMode was changed to another mode");
                                        await NavigateBackToDeviceView();
                                    }
                                }
                                break;
                            case MoCoBusProgramMode.Invalid:
                                if (navService.CurrentPageKey != ViewModelLocator.DeviceViewKey)
                                {
                                    System.Diagnostics.Debug.WriteLine("Navigating back to DeviceView since ProgramMode was changed to invalid mode");
                                    await NavigateBackToDeviceView();
                                }
                                break;
                        }
                    });
            _programModeBinding.ForceUpdateValueFromSourceToTarget();

            base.ViewDidLoad();
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

        private async Task NavigateBackToDeviceView()
        {
            var navService = ServiceLocator.Current.GetInstance<INavigationService>();

            while (navService.CurrentPageKey != NavigationService.RootPageKey 
                && navService.CurrentPageKey != NavigationService.UnknownPageKey 
                && navService.CurrentPageKey != ViewModelLocator.DeviceViewKey)
            {
                navService.GoBack();
                await Task.Delay(100);
            }
        }

        /*public override void ViewWillUnload()
        {
            
        }*/

        /*public override void ViewWillDisappear(bool animated)
        {
            if (IsMovingFromParentViewController || IsBeingDismissed)
            {
                Vm.IsConnected = false;
            }
            
            base.ViewWillDisappear(animated);
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            if (IsMovingFromParentViewController || IsBeingDismissed)
            {
                Vm.IsConnected = false;
            }

            base.WillMoveToParentViewController(parent);
        }*/

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            System.Diagnostics.Debug.WriteLine("DeviceViewController RowSelected");

            _modeChangeRequested = true;

            System.Diagnostics.Debug.WriteLine(tableView.CellAt(indexPath));

            switch (indexPath.Section)
            {
                case 0:
                    {
                        switch (indexPath.Row)
                        {
                            case 0: // sms
                                Vm.SetModeSmsCommand.Execute(null);
                                break;
                            case 1: // pano
                                //Vm.SetModePanoCommand.Execute(null);
                                break;
                            case 2: // astro
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
