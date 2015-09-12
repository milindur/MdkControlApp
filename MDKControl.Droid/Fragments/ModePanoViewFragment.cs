using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid.Fragments
{
    public class ModePanoViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
        private Binding _delayTimeBinding;
        
        private Button _setStartButton;
        private Button _setStopButton;
        private Button _swapStartStopButton;
        private Button _setRefStartButton;
        private Button _setRefStopButton;
        private Button _startProgramButton;

        private EditText _exposureTimeEditText;
        private EditText _delayTimeEditText;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModePanoView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ExposureTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Exposure", Vm.ExposureTime);
                    dlg.Closed += (oo, ee) => { Vm.ExposureTime = ee; };
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            DelayTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Delay", Vm.DelayTime);
                    dlg.Closed += (oo, ee) => { Vm.DelayTime = ee; };
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            SetStartButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Start");
                    dlg.SetCommand("Closed", Vm.SetStartCommand);
                    dlg.Show(FragmentManager, "joystickDlg");
                };
            SetStopButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Stop");
                    dlg.SetCommand("Closed", Vm.SetStopCommand);
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            SwapStartStopButton.Click += (o, e) => {};
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);

            SetRefStartButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Fov Start");
                    dlg.SetCommand("Closed", Vm.SetRefStartCommand);
                    dlg.Show(FragmentManager, "joystickDlg");
                };
            SetRefStopButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Fov Stop");
                    dlg.SetCommand("Closed", Vm.SetRefStopCommand);
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            StartProgramButton.Click += (o, e) => 
                {  
                    var dlg = ModePanoStatusViewFragment.NewInstance();
                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                    dlg.SetCommand("Resumed", Vm.StartProgramCommand);
                    dlg.Show(FragmentManager, "statusDlg");
                };
            StartProgramButton.SetCommand("Click", Vm.StartProgramCommand);
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);

            _activity = activity;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _activity = null;
        }

        public override void OnResume()
        {
            base.OnResume();

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus)
                .WhenSourceChanges(() =>
                    {
                        if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped)
                        {
                            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>("statusDlg");
                            if (dlg == null)
                            {
                                dlg = ModePanoStatusViewFragment.NewInstance();
                                dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                                dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                                dlg.SetCommand("Resumed", Vm.StartProgramCommand);
                                dlg.Show(FragmentManager, "statusDlg");
                                DeviceVm.StartUpdateTask();
                            }
                        }
                    });

            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime)
                .WhenSourceChanges(() =>
                    { 
                        ExposureTimeEditText.Text = string.Format("{0:F1}s", Vm.ExposureTime); 
                    });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime)
                .WhenSourceChanges(() =>
                    { 
                        DelayTimeEditText.Text = string.Format("{0:F1}s", Vm.DelayTime); 
                    });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();
        }

        public override void OnPause()
        {
            _runStatusBinding.Detach();
            _exposureTimeBinding.Detach();
            _delayTimeBinding.Detach();

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>("statusDlg");
            if (dlg != null)
            {
                dlg.Dismiss();
            }
            dlg = FragmentManager.FindFragmentByTag<DialogFragment>("joystickDlg");
            if (dlg != null)
            {
                dlg.Dismiss();
            }

            base.OnPause();
        }

        public ModePanoViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)_activity).Vm.ModePanoViewModel;
            }
        }

        public DeviceViewModel DeviceVm
        {
            get
            {
                return ((DeviceViewActivity)_activity).Vm;
            }
        }

        public Button SetStartButton
        {
            get 
            {
                return _setStartButton
                    ?? (_setStartButton = View.FindViewById<Button>(Resource.Id.SetStart));
            }
        }

        public Button SetStopButton
        {
            get 
            {
                return _setStopButton
                    ?? (_setStopButton = View.FindViewById<Button>(Resource.Id.SetStop));
            }
        }

        public Button SwapStartStopButton
        {
            get 
            {
                return _swapStartStopButton
                    ?? (_swapStartStopButton = View.FindViewById<Button>(Resource.Id.SwapStartStop));
            }
        }

        public Button SetRefStartButton
        {
            get 
            {
                return _setRefStartButton
                    ?? (_setRefStartButton = View.FindViewById<Button>(Resource.Id.SetRefStart));
            }
        }

        public Button SetRefStopButton
        {
            get 
            {
                return _setRefStopButton
                    ?? (_setRefStopButton = View.FindViewById<Button>(Resource.Id.SetRefStop));
            }
        }

        public Button StartProgramButton
        {
            get
            {
                return _startProgramButton
                    ?? (_startProgramButton = View.FindViewById<Button>(Resource.Id.StartProgram));
            }
        }

        public EditText ExposureTimeEditText
        {
            get
            {
                return _exposureTimeEditText
                    ?? (_exposureTimeEditText = View.FindViewById<EditText>(Resource.Id.ExposureTime));
            }
        }

        public EditText DelayTimeEditText
        {
            get
            {
                return _delayTimeEditText
                    ?? (_delayTimeEditText = View.FindViewById<EditText>(Resource.Id.PostDelayTime));
            }
        }
    }
}
    