using System;
using Android.App;
using Android.OS;
using Android.Widget;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using GalaSoft.MvvmLight.Helpers;
using Android.Views;
using Microsoft.Practices.ServiceLocation;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid
{
    [Activity()]
    public class DeviceViewActivity : ActivityBaseEx
    {
        private Binding _isConnectedBinding;
        private Binding _showDisconnectedBinding;
        private Binding _showConnectingBinding;
        private Binding _showConnectedBinding;

        private ViewGroup _disconnectedLayout;
        private ViewGroup _connectingLayout;
        private ViewGroup _connectedLayout;

        private Switch _connectSwitch;
        private JoystickView _joystick;

        public DeviceViewActivity()
        {
        }

        public DeviceViewModel Vm { get; private set; }

        public Switch ConnectSwitch
        {
            get
            {
                return _connectSwitch
                    ?? (_connectSwitch = FindViewById<Switch>(Resource.Id.ConnectSwitch));
            }
        }

        public ViewGroup DisconnectedLayout
        {
            get 
            {
                return _disconnectedLayout
                    ?? (_disconnectedLayout = FindViewById<ViewGroup>(Resource.Id.DisconnectedLayout));
            }
        }

        public ViewGroup ConnectingLayout
        {
            get 
            {
                return _connectingLayout
                    ?? (_connectingLayout = FindViewById<ViewGroup>(Resource.Id.ConnectingLayout));
            }
        }

        public ViewGroup ConnectedLayout
        {
            get 
            {
                return _connectedLayout
                    ?? (_connectedLayout = FindViewById<ViewGroup>(Resource.Id.ConnectedLayout));
            }
        }

        public JoystickView Joystick
        {
            get 
            {
                return _joystick
                    ?? (_joystick = FindViewById<JoystickView>(Resource.Id.Joystick));
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DeviceView);

            Vm = GlobalNavigation.GetAndRemoveParameter<DeviceViewModel>(Intent);
        }

        protected override void OnResume()
        {
            base.OnResume();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectSwitch.Checked, BindingMode.TwoWay);
            _showDisconnectedBinding = this.SetBinding(() => Vm.IsDisconnected, () => DisconnectedLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectingBinding = this.SetBinding(() => Vm.IsConnecting, () => ConnectingLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectedLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);

            Joystick.JoystickMove.SetCommand(Vm.JoystickCommand);
        }

        protected override void OnPause()
        {
            _isConnectedBinding.Detach();
            _showDisconnectedBinding.Detach();
            _showConnectingBinding.Detach();
            _showConnectedBinding.Detach();

            Vm.IsConnected = false;

            base.OnPause();
        }
    }
}
