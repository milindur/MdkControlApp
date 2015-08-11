using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Fragments;
using MDKControl.Droid.Helpers;
using MDKControl.Droid.Widgets;
using Microsoft.Practices.ServiceLocation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using MDKControl.Core.Models;

namespace MDKControl.Droid.Activities
{
    [Activity(Label = "Device", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DeviceViewActivity : ActivityBaseEx
    {
        private Binding _isConnectedBinding;
        private Binding _showDisconnectedBinding;
        private Binding _showConnectingBinding;
        private Binding _showConnectedBinding;
        private Binding _programModeBinding;

        private ViewGroup _disconnectedLayout;
        private ViewGroup _connectingLayout;
        private ViewGroup _connectedLayout;

        private Switch _connectSwitch;

        private Button _setModeSmsButton;
        private Button _setModePanoButton;
        private Button _setModeAstroButton;

        public DeviceViewActivity()
        {
        }

        public DeviceViewModel Vm { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DeviceView);

            Vm = GlobalNavigation.GetAndRemoveParameter<DeviceViewModel>(Intent);

            SetModeSmsButton.Click += (o, e) => {};
            SetModeSmsButton.SetCommand("Click", Vm.SetModeSmsCommand);
            SetModePanoButton.Click += (o, e) => {};
            SetModePanoButton.SetCommand("Click", Vm.SetModePanoCommand);
            SetModeAstroButton.Click += (o, e) => {};
            SetModeAstroButton.SetCommand("Click", Vm.SetModeAstroCommand);

            ShowModeSmsFragment();
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

            _programModeBinding = this.SetBinding(() => Vm.ProgramMode)
                .WhenSourceChanges(OnProgramModeChanged);
        }

        protected override void OnPause()
        {
            _isConnectedBinding.Detach();
            _showDisconnectedBinding.Detach();
            _showConnectingBinding.Detach();
            _showConnectedBinding.Detach();

            _programModeBinding.Detach();

            Vm.StopJoystick();

            base.OnPause();
        }

        protected override void OnApplicationStop()
        {
            Vm.Cleanup();

            base.OnApplicationStop();
        }

        private void ShowModeSmsFragment()
        {
            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModeSmsViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModeSmsButton.Enabled = false;
        }

        private void ShowModeAstroFragment()
        {
            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModeAstroViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModeAstroButton.Enabled = false;
        }

        private void ShowModePanoFragment()
        {
            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModePanoViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModePanoButton.Enabled = false;
        }

        private void OnProgramModeChanged()
        {
            SetModeAstroButton.Enabled = true;
            SetModePanoButton.Enabled = true;
            SetModeSmsButton.Enabled = true;

            if (Vm.ProgramMode == MoCoBusProgramMode.ShootMoveShoot)
            {
                ShowModeSmsFragment();
            }
            else if (Vm.ProgramMode == MoCoBusProgramMode.Astro)
            {
                ShowModeAstroFragment();
            }
            else if (Vm.ProgramMode == MoCoBusProgramMode.Panorama)
            {
                ShowModePanoFragment();
            }
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

        public Button SetModeSmsButton
        {
            get 
            {
                return _setModeSmsButton
                    ?? (_setModeSmsButton = FindViewById<Button>(Resource.Id.SetModeSms));
            }
        }

        public Button SetModePanoButton
        {
            get 
            {
                return _setModePanoButton
                    ?? (_setModePanoButton = FindViewById<Button>(Resource.Id.SetModePano));
            }
        }

        public Button SetModeAstroButton
        {
            get 
            {
                return _setModeAstroButton
                    ?? (_setModeAstroButton = FindViewById<Button>(Resource.Id.SetModeAstro));
            }
        }
    }
}
