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
using MDKControl.Core.Models;

namespace MDKControl.Droid.Fragments
{
    public class ModeSmsViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;

        private Binding _exposureTimeBinding;
        private Binding _delayTimeBinding;
        private Binding _intervalTimeBinding;
        private Binding _durationTimeBinding;
        private Binding _maxShotsBinding;

        private Binding _sliderStartPosBinding;
        private Binding _sliderStopPosBinding;
        private Binding _panStartPosBinding;
        private Binding _panStopPosBinding;
        private Binding _tiltStartPosBinding;
        private Binding _tiltStopPosBinding;

        private Button _setStartButton;
        private Button _setStopButton;
        private Button _swapStartStopButton;
        private Button _startProgramButton;

        private EditText _exposureTimeEditText;
        private EditText _delayTimeEditText;
        private EditText _intervalTimeEditText;
        private EditText _durationTimeEditText;
        private EditText _maxShotsEditText;

        private EditText _sliderStartPosEditText;
        private EditText _sliderStopPosEditText;
        private EditText _panStartPosEditText;
        private EditText _panStopPosEditText;
        private EditText _tiltStartPosEditText;
        private EditText _tiltStopPosEditText;

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

            IntervalTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Interval", Vm.IntervalTime);
                    dlg.Closed += (oo, ee) => { System.Diagnostics.Debug.WriteLine("Setting IntervalTime from dialog"); Vm.IntervalTime = ee; };
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            DurationTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Duration", Vm.DurationTime);
                    dlg.Closed += (oo, ee) => { Vm.DurationTime = ee; };
                    dlg.Show(FragmentManager, "joystickDlg");
                };

            MaxShotsEditText.Click += (o, e) => 
                {
                    var dlg = IntegerViewFragment.NewInstance("Shots", Vm.MaxShots);
                    dlg.Closed += (oo, ee) => { Vm.MaxShots = (ushort)ee; };
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

            StartProgramButton.Click += (o, e) => 
                {  
                    var dlg = ModeSmsStatusViewFragment.NewInstance();
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
                                dlg = ModeSmsStatusViewFragment.NewInstance();
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

            _intervalTimeBinding = this.SetBinding(() => Vm.IntervalTime)
                .WhenSourceChanges(() =>
                    {
                        IntervalTimeEditText.Text = string.Format("{0:F1}s", Vm.IntervalTime);
                    });
            _intervalTimeBinding.ForceUpdateValueFromSourceToTarget();

            _durationTimeBinding = this.SetBinding(() => Vm.DurationTime)
                .WhenSourceChanges(() =>
                    {
                        DurationTimeEditText.Text = string.Format("{0}:{1:00}m", (int)(Vm.DurationTime / 60), (int)Vm.DurationTime % 60);
                    });
            _durationTimeBinding.ForceUpdateValueFromSourceToTarget();

            _maxShotsBinding = this.SetBinding(() => Vm.MaxShots)
                .WhenSourceChanges(() =>
                    {
                        MaxShotsEditText.Text = string.Format("{0}", Vm.MaxShots);
                    });
            _maxShotsBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStartPosBinding = this.SetBinding(() => Vm.SliderStartPosition)
                .WhenSourceChanges(() =>
                    {
                        SliderStartPosEditText.Text = string.Format("{0}", Vm.SliderStartPosition);
                    });
            _sliderStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStopPosBinding = this.SetBinding(() => Vm.SliderStopPosition)
                .WhenSourceChanges(() =>
                    {
                        SliderStopPosEditText.Text = string.Format("{0}", Vm.SliderStopPosition);
                    });
            _sliderStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStartPosBinding = this.SetBinding(() => Vm.PanStartPosition)
                .WhenSourceChanges(() =>
                    {
                        PanStartPosEditText.Text = string.Format("{0:F1}°", (double)Vm.PanStartPosition / (190 * 200 * 16) * 360);
                    });
            _panStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStopPosBinding = this.SetBinding(() => Vm.PanStopPosition)
                .WhenSourceChanges(() =>
                    {
                        PanStopPosEditText.Text = string.Format("{0:F1}°", (double)Vm.PanStopPosition / (190 * 200 * 16) * 360);
                    });
            _panStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStartPosBinding = this.SetBinding(() => Vm.TiltStartPosition)
                .WhenSourceChanges(() =>
                    {
                        TiltStartPosEditText.Text = string.Format("{0:F1}°", (double)Vm.TiltStartPosition / (190 * 200 * 16) * 360);
                    });
            _tiltStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStopPosBinding = this.SetBinding(() => Vm.TiltStopPosition)
                .WhenSourceChanges(() =>
                    {
                        TiltStopPosEditText.Text = string.Format("{0:F1}°", (double)Vm.TiltStopPosition / (190 * 200 * 16) * 360);
                    });
            _tiltStopPosBinding.ForceUpdateValueFromSourceToTarget();
        }

        public override void OnPause()
        {
            _runStatusBinding.Detach();

            _exposureTimeBinding.Detach();
            _delayTimeBinding.Detach();
            _intervalTimeBinding.Detach();
            _durationTimeBinding.Detach();
            _maxShotsBinding.Detach();

            _sliderStartPosBinding.Detach();
            _sliderStopPosBinding.Detach();
            _panStartPosBinding.Detach();
            _panStopPosBinding.Detach();
            _tiltStartPosBinding.Detach();
            _tiltStopPosBinding.Detach();

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

        public ModeSmsViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)_activity).Vm.ModeSmsViewModel;
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

        public EditText MaxShotsEditText
        {
            get
            {
                return _maxShotsEditText
                    ?? (_maxShotsEditText = View.FindViewById<EditText>(Resource.Id.MaxShots));
            }
        }

        public EditText SliderStartPosEditText
        {
            get
            {
                return _sliderStartPosEditText
                    ?? (_sliderStartPosEditText = View.FindViewById<EditText>(Resource.Id.SliderStartPos));
            }
        }

        public EditText SliderStopPosEditText
        {
            get
            {
                return _sliderStopPosEditText
                    ?? (_sliderStopPosEditText = View.FindViewById<EditText>(Resource.Id.SliderStopPos));
            }
        }

        public EditText PanStartPosEditText
        {
            get
            {
                return _panStartPosEditText
                    ?? (_panStartPosEditText = View.FindViewById<EditText>(Resource.Id.PanStartPos));
            }
        }

        public EditText PanStopPosEditText
        {
            get
            {
                return _panStopPosEditText
                    ?? (_panStopPosEditText = View.FindViewById<EditText>(Resource.Id.PanStopPos));
            }
        }

        public EditText TiltStartPosEditText
        {
            get
            {
                return _tiltStartPosEditText
                    ?? (_tiltStartPosEditText = View.FindViewById<EditText>(Resource.Id.TiltStartPos));
            }
        }

        public EditText TiltStopPosEditText
        {
            get
            {
                return _tiltStopPosEditText
                    ?? (_tiltStopPosEditText = View.FindViewById<EditText>(Resource.Id.TiltStopPos));
            }
        }
    }
}
