using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;

namespace MDKControl.iOS
{
    public abstract class SubDeviceTableViewControllerBase : UITableViewController
    {
        private Binding _isConnectedBinding;
        private Binding _programModeBinding;

        private readonly MoCoBusProgramMode _programMode = MoCoBusProgramMode.Invalid;
        private readonly string _viewPageKey = null;

        public SubDeviceTableViewControllerBase(IntPtr handle, MoCoBusProgramMode programMode, string viewPageKey)
            : base(handle)
        {
            _programMode = programMode;
            _viewPageKey = viewPageKey;
        }

        public abstract DeviceViewModel DeviceVm { get; }

        protected virtual void SetupBindings()
        {
            _isConnectedBinding = this.SetBinding(() => DeviceVm.IsConnected).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("SubDeviceViewControllerBase: DeviceViewModel PropertyChanged IsConnected");

                    var navService = ServiceLocator.Current.GetInstance<INavigationService>();
                    if (!DeviceVm.IsConnected && navService.CurrentPageKey == _viewPageKey)
                    {
                        System.Diagnostics.Debug.WriteLine("Navigating back since device is not connected anymore.");
                        navService.GoBack();
                    }
                });
            _isConnectedBinding.ForceUpdateValueFromSourceToTarget();

            _programModeBinding = this.SetBinding(() => DeviceVm.ProgramMode).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("SubDeviceViewControllerBase: DeviceViewModel PropertyChanged ProgramMode");

                    var navService = ServiceLocator.Current.GetInstance<INavigationService>();
                    if (_programMode != MoCoBusProgramMode.Invalid && DeviceVm.ProgramMode != _programMode && navService.CurrentPageKey == _viewPageKey)
                    {
                        System.Diagnostics.Debug.WriteLine("Navigating back since ProgramMode was changed to another mode");
                        navService.GoBack();
                    }
                });
            _programModeBinding.ForceUpdateValueFromSourceToTarget();
        }

        protected virtual void DetachBindings()
        {
            _isConnectedBinding?.Detach();
            _isConnectedBinding = null;

            _programModeBinding?.Detach();
            _programModeBinding = null;
        }
    }
}
