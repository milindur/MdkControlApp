using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using MDKControl.Droid.Widgets;
using Microsoft.Practices.ServiceLocation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid
{
    [Activity(Label = "Device", ScreenOrientation = ScreenOrientation.Portrait)]
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
        private Button _setStartButton;
        private Button _setStopButton;
        private Button _swapStartStopButton;
        private Button _gotoStartButton;
        private Button _gotoStopButton;
        private Button _startProgramButton;

        public DeviceViewActivity()
        {
        }

        public DeviceViewModel Vm { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DeviceView);

            Vm = GlobalNavigation.GetAndRemoveParameter<DeviceViewModel>(Intent);

            GotoStartButton.Click += (o, e) => {};
            GotoStartButton.SetCommand("Click", Vm.GotoStartCommand);
            GotoStopButton.Click += (o, e) => {};
            GotoStopButton.SetCommand("Click", Vm.GotoStopCommand);
            StartProgramButton.Click += (o, e) => {};
            StartProgramButton.SetCommand("Click", Vm.StartProgramCommand);

            SetStartButton.Click += (o, e) => {};
            SetStartButton.SetCommand("Click", Vm.SetStartCommand);
            SetStopButton.Click += (o, e) => {};
            SetStopButton.SetCommand("Click", Vm.SetStopCommand);
            SwapStartStopButton.Click += (o, e) => {};
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);
        }

        protected override void OnResume()
        {
            base.OnResume();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            DisconnectedLayout.Visibility = ViewStates.Visible;
            ConnectingLayout.Visibility = ViewStates.Gone;
            ConnectedLayout.Visibility = ViewStates.Gone;

            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectSwitch.Checked, BindingMode.TwoWay);
            _showDisconnectedBinding = this.SetBinding(() => Vm.IsDisconnected, () => DisconnectedLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectingBinding = this.SetBinding(() => Vm.IsConnecting, () => ConnectingLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectedLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);

            Joystick.JoystickStart.SetCommand(Vm.StartJoystickCommand);
            Joystick.JoystickStop.SetCommand(Vm.StopJoystickCommand);
            Joystick.JoystickMove.SetCommand(Vm.MoveJoystickCommand);
        }

        protected override void OnPause()
        {
            _isConnectedBinding.Detach();
            _showDisconnectedBinding.Detach();
            _showConnectingBinding.Detach();
            _showConnectedBinding.Detach();

            Vm.StopJoystick(null);

            base.OnPause();
        }

        protected override void OnApplicationStop()
        {
            Vm.Cleanup();

            base.OnApplicationStop();
        }

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

        public Button GotoStartButton
        {
            get 
            {
                return _gotoStartButton
                    ?? (_gotoStartButton = FindViewById<Button>(Resource.Id.GotoStart));
            }
        }

        public Button GotoStopButton
        {
            get 
            {
                return _gotoStopButton
                    ?? (_gotoStopButton = FindViewById<Button>(Resource.Id.GotoStop));
            }
        }

        public Button StartProgramButton
        {
            get 
            {
                return _startProgramButton
                    ?? (_startProgramButton = FindViewById<Button>(Resource.Id.StartProgram));
            }
        }

        public Button SetStartButton
        {
            get 
            {
                return _setStartButton
                    ?? (_setStartButton = FindViewById<Button>(Resource.Id.SetStart));
            }
        }

        public Button SetStopButton
        {
            get 
            {
                return _setStopButton
                    ?? (_setStopButton = FindViewById<Button>(Resource.Id.SetStop));
            }
        }

        public Button SwapStartStopButton
        {
            get 
            {
                return _swapStartStopButton
                    ?? (_swapStartStopButton = FindViewById<Button>(Resource.Id.SwapStartStop));
            }
        }
    }
}
