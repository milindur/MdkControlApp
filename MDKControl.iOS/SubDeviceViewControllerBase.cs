using System;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using UIKit;

namespace MDKControl.iOS
{
    public abstract class SubDeviceViewControllerBase : UIViewController
    {
        private Binding _isConnectedBinding;
        private Binding _programModeBinding;

        private readonly MoCoBusProgramMode _programMode;
        private readonly string _viewPageKey;

        protected SubDeviceViewControllerBase(IntPtr handle, MoCoBusProgramMode programMode, string viewPageKey)
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
                    System.Diagnostics.Debug.WriteLine($"SubDeviceViewControllerBase({this.ToString()}): DeviceViewModel PropertyChanged IsConnected {DeviceVm.IsConnected}");

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
                    System.Diagnostics.Debug.WriteLine($"SubDeviceViewControllerBase({this.ToString()}): DeviceViewModel PropertyChanged ProgramMode {DeviceVm.ProgramMode}");

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
