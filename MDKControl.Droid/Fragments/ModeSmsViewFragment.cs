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
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Widgets;
using MDKControl.Droid.Activities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid.Fragments
{
    public class ModeSmsViewFragment : Fragment
    {
        private Button _setStartButton;
        private Button _setStopButton;
        private Button _swapStartStopButton;
        private Button _startProgramButton;
        private Button _pauseProgramButton;
        private Button _stopProgramButton;

        private EditText _exposureTimeEditText;
        private EditText _intervalTimeEditText;
        private EditText _durationTimeEditText;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModeSmsView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ExposureTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Exposure", (int)Vm.ExposureTime);
                    dlg.Closed += (oo, ee) => { Vm.ExposureTime = ee; };
                    dlg.Show(FragmentManager, "dlg");
                };
            
            IntervalTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Interval", (int)Vm.IntervalTime);
                    dlg.Closed += (oo, ee) => { Vm.IntervalTime = ee; };
                    dlg.Show(FragmentManager, "dlg");
                };

            DurationTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Duration", (int)Vm.DurationTime);
                    dlg.Closed += (oo, ee) => { Vm.DurationTime = ee; };
                    dlg.Show(FragmentManager, "dlg");
                };

            SetStartButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Start");
                    dlg.SetCommand("Closed", Vm.SetStartCommand);
                    dlg.Show(FragmentManager, "dlg");
                };
            SetStopButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Stop");
                    dlg.SetCommand("Closed", Vm.SetStopCommand);
                    dlg.Show(FragmentManager, "dlg");
                };

            SwapStartStopButton.Click += (o, e) => {};
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);

            StartProgramButton.Click += (o, e) => {};
            StartProgramButton.SetCommand("Click", Vm.StartProgramCommand);
            PauseProgramButton.Click += (o, e) => {};
            PauseProgramButton.SetCommand("Click", Vm.PauseProgramCommand);
            StopProgramButton.Click += (o, e) => {};
            StopProgramButton.SetCommand("Click", Vm.StopProgramCommand);

            this.SetBinding(() => Vm.ExposureTime, () => ExposureTimeEditText.Text, BindingMode.OneWay)
                .ConvertSourceToTarget(f => string.Format("{0:F0} s", f))
                .ForceUpdateValueFromSourceToTarget();

            this.SetBinding(() => Vm.IntervalTime, () => IntervalTimeEditText.Text, BindingMode.OneWay)
                .ConvertSourceToTarget(f => string.Format("{0:F0} s", f))
                .ForceUpdateValueFromSourceToTarget();

            this.SetBinding(() => Vm.DurationTime, () => DurationTimeEditText.Text, BindingMode.OneWay)
                .ConvertSourceToTarget(f => string.Format("{0:F1} m", f / 60.0f))
                .ForceUpdateValueFromSourceToTarget();
        }

        public ModeSmsViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)Activity).Vm.ModeSmsViewModel;
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

        public Button StartProgramButton
        {
            get
            {
                return _startProgramButton
                    ?? (_startProgramButton = View.FindViewById<Button>(Resource.Id.StartProgram));
            }
        }

        public Button PauseProgramButton
        {
            get
            {
                return _pauseProgramButton
                    ?? (_pauseProgramButton = View.FindViewById<Button>(Resource.Id.PauseProgram));
            }
        }

        public Button StopProgramButton
        {
            get
            {
                return _stopProgramButton
                    ?? (_stopProgramButton = View.FindViewById<Button>(Resource.Id.StopProgram));
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

        public EditText IntervalTimeEditText
        {
            get
            {
                return _intervalTimeEditText
                    ?? (_intervalTimeEditText = View.FindViewById<EditText>(Resource.Id.IntervalTime));
            }
        }

        public EditText DurationTimeEditText
        {
            get
            {
                return _durationTimeEditText
                    ?? (_durationTimeEditText = View.FindViewById<EditText>(Resource.Id.DurationTime));
            }
        }
    }
}
    