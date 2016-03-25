using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Fragments;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;

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

        private MoCoBusProgramMode _programMode = MoCoBusProgramMode.Invalid;

        public DeviceViewActivity()
        {
        }

        public DeviceViewModel Vm { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DeviceView);

            App.Initialize(this);

            Vm = GlobalNavigation.GetAndRemoveParameter<DeviceViewModel>(Intent);

            SetModeSmsButton.Click += (o, e) => {};
            SetModeSmsButton.SetCommand("Click", Vm.SetModeSmsCommand);
            SetModePanoButton.Click += (o, e) => {};
            SetModePanoButton.SetCommand("Click", Vm.SetModePanoCommand);
            SetModeAstroButton.Click += (o, e) => {};
            SetModeAstroButton.SetCommand("Click", Vm.SetModeAstroCommand);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            base.OnResume();

            App.Initialize(this);
            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            DisconnectedLayout.Visibility = ViewStates.Visible;
            ConnectingLayout.Visibility = ViewStates.Gone;
            ConnectedLayout.Visibility = ViewStates.Gone;
        
            _isConnectedBinding = this.SetBinding(() => Vm.IsConnected, () => ConnectSwitch.Checked, BindingMode.TwoWay);
            _showDisconnectedBinding = this.SetBinding(() => Vm.IsDisconnected, () => DisconnectedLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectingBinding = this.SetBinding(() => Vm.IsConnecting, () => ConnectingLayout.Visibility)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Gone);
            _showConnectedBinding = this.SetBinding(() => Vm.IsConnected)
                .WhenSourceChanges(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed");

                        ConnectedLayout.Visibility = Vm.IsConnected ? ViewStates.Visible : ViewStates.Gone;

                        if (!Vm.IsConnected)
                        {
                            _programMode = MoCoBusProgramMode.Invalid;
                            
                            System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: Search dialogs and close them");

                            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
                            if (dlg != null)
                            {
                                System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: Found dialog, dismiss it");
                                dlg.DismissAllowingStateLoss();
                            }

                            System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: Search view fragment");
                            var f = FragmentManager.FindFragmentById<Fragment>(Resource.Id.ConnectedFragmentContainer);
                            if (f != null)
                            {
                                System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: Remove view fragment");
                                var ft = FragmentManager.BeginTransaction();
                                ft.Remove(f);
                                ft.SetTransition(FragmentTransit.FragmentFade);
                                ft.Commit();
                            }

                            System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: PopBackStackImmediate");
                            FragmentManager.PopBackStackImmediate(null, PopBackStackFlags.Inclusive);

                            System.Diagnostics.Debug.WriteLine("DeviceViewActivity IsConnected Changed: ExecutePendingTransactions");
                            FragmentManager.ExecutePendingTransactions();
                        }
                    });

            _programMode = MoCoBusProgramMode.Invalid;
            _programModeBinding = this.SetBinding(() => Vm.ProgramMode)
                .WhenSourceChanges(OnProgramModeChanged);
            _programModeBinding.ForceUpdateValueFromSourceToTarget();
        }

        protected override void OnPause()
        {
            _isConnectedBinding?.Detach();
            _showDisconnectedBinding?.Detach();
            _showConnectingBinding?.Detach();
            _showConnectedBinding?.Detach();
            _programModeBinding?.Detach();

            Vm.StopUpdateTask();
            Vm.JoystickViewModel.StopJoystick(null);
            Vm.JoystickViewModel.StopSlider(null);

            base.OnPause();
        }

        protected override void OnApplicationStop()
        {
            Vm.Cleanup();

            base.OnApplicationStop();
        }

        private void ShowModeSmsFragment()
        {
            System.Diagnostics.Debug.WriteLine("ShowModeSmsFragment");

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            if (dlg != null)
            {
                dlg.DismissAllowingStateLoss();
            }

            FragmentManager.ExecutePendingTransactions();
            FragmentManager.PopBackStackImmediate(null, PopBackStackFlags.Inclusive);

            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModeSmsViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModeSmsButton.Enabled = false;
        }

        private void ShowModeAstroFragment()
        {
            System.Diagnostics.Debug.WriteLine("ShowModeAstroFragment");

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            if (dlg != null)
            {
                dlg.DismissAllowingStateLoss();
            }

            FragmentManager.ExecutePendingTransactions();
            FragmentManager.PopBackStackImmediate(null, PopBackStackFlags.Inclusive);

            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModeAstroViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModeAstroButton.Enabled = false;
        }

        private void ShowModePanoFragment()
        {
            System.Diagnostics.Debug.WriteLine("ShowModePanoFragment");

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            if (dlg != null)
            {
                dlg.DismissAllowingStateLoss();
            }

            FragmentManager.ExecutePendingTransactions();
            FragmentManager.PopBackStackImmediate(null, PopBackStackFlags.Inclusive);

            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.ConnectedFragmentContainer, new ModePanoViewFragment());
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.Commit();

            SetModePanoButton.Enabled = false;
        }

        private void OnProgramModeChanged()
        {
            if (!Vm.IsConnected)
                return;
            
            if (_programMode == Vm.ProgramMode)
                return;

            _programMode = Vm.ProgramMode;

            System.Diagnostics.Debug.WriteLine("OnProgramModeChanged: {0}", _programMode);

            SetModeAstroButton.Enabled = true;
            SetModePanoButton.Enabled = true;
            SetModeSmsButton.Enabled = true;

            if (_programMode == MoCoBusProgramMode.ShootMoveShoot)
            {
                ShowModeSmsFragment();
            }
            else if (_programMode == MoCoBusProgramMode.Astro)
            {
                ShowModeAstroFragment();
            }
            else if (_programMode == MoCoBusProgramMode.Panorama)
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
