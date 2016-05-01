using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;

namespace MDKControl.Droid.Fragments
{
    public class ModeSmsViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private readonly object _runStatusLock = new object();
        private MoCoBusRunStatus _prevRunStatus = MoCoBusRunStatus.Stopped;

        private Binding _exposureTimeBinding;
        private Binding _preDelayTimeBinding;
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
        private EditText _preDelayTimeEditText;
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnCreateView");

            return inflater.Inflate(Resource.Layout.ModeSmsView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnActivityCreated");

            Vm.PropertyChanged += (o, e) => { };

            ExposureTimeEditText.Click += (o, e) =>
                {
                    var dlg = TimeViewFragment.NewInstance("Exposure", Vm.ExposureTime);
                    dlg.Closed += (oo, ee) => { Vm.ExposureTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            PreDelayTimeEditText.Click += (o, e) =>
                {
                    var dlg = TimeViewFragment.NewInstance("Pre-Delay", Vm.PreDelayTime);
                    dlg.Closed += (oo, ee) => { Vm.PreDelayTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            DelayTimeEditText.Click += (o, e) =>
                {
                    var dlg = TimeViewFragment.NewInstance("Delay", Vm.DelayTime);
                    dlg.Closed += (oo, ee) => { Vm.DelayTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            IntervalTimeEditText.Click += (o, e) =>
                {
                    var dlg = TimeViewFragment.NewInstance("Interval", Vm.IntervalTime);
                    dlg.Closed += (oo, ee) => { System.Diagnostics.Debug.WriteLine("Setting IntervalTime from dialog"); Vm.IntervalTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            DurationTimeEditText.Click += (o, e) =>
                {
                    var dlg = TimeViewFragment.NewInstance("Duration", Vm.DurationTime);
                    dlg.Closed += (oo, ee) => { Vm.DurationTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            MaxShotsEditText.Click += (o, e) =>
                {
                    var dlg = IntegerViewFragment.NewInstance("Shots", Vm.MaxShots);
                    dlg.Closed += (oo, ee) => { Vm.MaxShots = (ushort)ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            SetStartButton.Click += (o, e) =>
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Start");
                    dlg.Closed += (oo, ee) => { };
                    dlg.SetCommand("Closed", Vm.SetStartCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };
            SetStopButton.Click += (o, e) =>
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Stop");
                    dlg.Closed += (oo, ee) => { };
                    dlg.SetCommand("Closed", Vm.SetStopCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            SwapStartStopButton.Click += (o, e) => { };
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);

            StartProgramButton.Click += (o, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment StartProgramButton Clicked");

                    var ft = FragmentManager.BeginTransaction();
                    ft.DisallowAddToBackStack();
                    var dlg = ModeSmsStatusViewFragment.NewInstance();
                    dlg.Stoped += (oo, ee) => { };
                    dlg.Paused += (oo, ee) => { };
                    dlg.Resumed += (oo, ee) => { };
                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                    dlg.SetCommand("Resumed", Vm.StartProgramCommand);
                    dlg.Show(ft, Consts.DialogTag);
                };
            StartProgramButton.SetCommand("Click", Vm.StartProgramCommand);
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);

            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnAttach");

            _activity = activity;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnDestroy");

            _activity = null;
        }

        public override void OnResume()
        {
            base.OnResume();

            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnResume");

            _prevRunStatus = MoCoBusRunStatus.Stopped;
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus)
                .WhenSourceChanges(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment RunStatus Changed (new={0},prev={1})", DeviceVm.RunStatus, _prevRunStatus);

                        lock (_runStatusLock)
                        {
                            if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped && DeviceVm.RunStatus != _prevRunStatus && !DeviceVm.IsUpdateTaskRunning)
                            {
                                System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment RunStatus Changed: Looking for dialog");

                                var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
                                if (dlg == null)
                                {
                                    System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment RunStatus Changed: Create ModeSmsStatusViewFragment");

                                    var ft = FragmentManager.BeginTransaction();
                                    ft.DisallowAddToBackStack();
                                    var dlg2 = ModeSmsStatusViewFragment.NewInstance();
                                    dlg2.Stoped += (oo, ee) => { };
                                    dlg2.Paused += (oo, ee) => { };
                                    dlg2.Resumed += (oo, ee) => { };
                                    dlg2.SetCommand("Stoped", Vm.StopProgramCommand);
                                    dlg2.SetCommand("Paused", Vm.PauseProgramCommand);
                                    dlg2.SetCommand("Resumed", Vm.StartProgramCommand);
                                    dlg2.Show(ft, Consts.DialogTag);
                                }
                                DeviceVm.StartUpdateTask();
                            }
                        }

                        _prevRunStatus = DeviceVm.RunStatus;
                    });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();

            _exposureTimeBinding = this.SetBinding(() => Vm.ExposureTime)
                .WhenSourceChanges(() =>
                    {
                        ExposureTimeEditText.Text = $"{Vm.ExposureTime:F1}s";
                    });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _preDelayTimeBinding = this.SetBinding(() => Vm.PreDelayTime)
                .WhenSourceChanges(() =>
                {
                    PreDelayTimeEditText.Text = $"{Vm.PreDelayTime:F1}s";
                });
            _preDelayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime)
                .WhenSourceChanges(() =>
                    {
                        DelayTimeEditText.Text = $"{Vm.DelayTime:F1}s";
                    });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();

            _intervalTimeBinding = this.SetBinding(() => Vm.IntervalTime)
                .WhenSourceChanges(() =>
                    {
                        IntervalTimeEditText.Text = $"{Vm.IntervalTime:F1}s";
                    });
            _intervalTimeBinding.ForceUpdateValueFromSourceToTarget();

            _durationTimeBinding = this.SetBinding(() => Vm.DurationTime)
                .WhenSourceChanges(() =>
                    {
                        DurationTimeEditText.Text = $"{(int)(Vm.DurationTime / 60)}:{(int)Vm.DurationTime % 60:00}m";
                    });
            _durationTimeBinding.ForceUpdateValueFromSourceToTarget();

            _maxShotsBinding = this.SetBinding(() => Vm.MaxShots)
                .WhenSourceChanges(() =>
                    {
                        MaxShotsEditText.Text = $"{Vm.MaxShots}";
                    });
            _maxShotsBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStartPosBinding = this.SetBinding(() => Vm.SliderStartPosition)
                .WhenSourceChanges(() =>
                    {
                        SliderStartPosEditText.Text = $"{Vm.SliderStartPosition}";
                    });
            _sliderStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _sliderStopPosBinding = this.SetBinding(() => Vm.SliderStopPosition)
                .WhenSourceChanges(() =>
                    {
                        SliderStopPosEditText.Text = $"{Vm.SliderStopPosition}";
                    });
            _sliderStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStartPosBinding = this.SetBinding(() => Vm.PanStartPosition)
                .WhenSourceChanges(() =>
                    {
                        PanStartPosEditText.Text = $"{(double)Vm.PanStartPosition / (190 * 200 * 16) * 360:F1}°";
                    });
            _panStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _panStopPosBinding = this.SetBinding(() => Vm.PanStopPosition)
                .WhenSourceChanges(() =>
                    {
                        PanStopPosEditText.Text = $"{(double)Vm.PanStopPosition / (190 * 200 * 16) * 360:F1}°";
                    });
            _panStopPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStartPosBinding = this.SetBinding(() => Vm.TiltStartPosition)
                .WhenSourceChanges(() =>
                    {
                        TiltStartPosEditText.Text = $"{(double)Vm.TiltStartPosition / (190 * 200 * 16) * 360:F1}°";
                    });
            _tiltStartPosBinding.ForceUpdateValueFromSourceToTarget();

            _tiltStopPosBinding = this.SetBinding(() => Vm.TiltStopPosition)
                .WhenSourceChanges(() =>
                    {
                        TiltStopPosEditText.Text = $"{(double)Vm.TiltStopPosition / (190 * 200 * 16) * 360:F1}°";
                    });
            _tiltStopPosBinding.ForceUpdateValueFromSourceToTarget();
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsViewFragment OnPause");

            _runStatusBinding?.Detach();

            _exposureTimeBinding?.Detach();
            _preDelayTimeBinding?.Detach();
            _delayTimeBinding?.Detach();
            _intervalTimeBinding?.Detach();
            _durationTimeBinding?.Detach();
            _maxShotsBinding?.Detach();

            _sliderStartPosBinding?.Detach();
            _sliderStopPosBinding?.Detach();
            _panStartPosBinding?.Detach();
            _panStopPosBinding?.Detach();
            _tiltStartPosBinding?.Detach();
            _tiltStopPosBinding?.Detach();

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            dlg?.DismissAllowingStateLoss();

            base.OnPause();
        }

        public ModeSmsViewModel Vm => ((DeviceViewActivity)_activity).Vm.ModeSmsViewModel;

        public DeviceViewModel DeviceVm => ((DeviceViewActivity)_activity).Vm;

        public Button SetStartButton => _setStartButton ?? (_setStartButton = View.FindViewById<Button>(Resource.Id.SetStart));

        public Button SetStopButton => _setStopButton ?? (_setStopButton = View.FindViewById<Button>(Resource.Id.SetStop));

        public Button SwapStartStopButton => _swapStartStopButton ?? (_swapStartStopButton = View.FindViewById<Button>(Resource.Id.SwapStartStop));

        public Button StartProgramButton => _startProgramButton ?? (_startProgramButton = View.FindViewById<Button>(Resource.Id.StartProgram));

        public EditText ExposureTimeEditText => _exposureTimeEditText ?? (_exposureTimeEditText = View.FindViewById<EditText>(Resource.Id.ExposureTime));

        public EditText PreDelayTimeEditText => _preDelayTimeEditText ?? (_preDelayTimeEditText = View.FindViewById<EditText>(Resource.Id.PreDelayTime));

        public EditText DelayTimeEditText => _delayTimeEditText ?? (_delayTimeEditText = View.FindViewById<EditText>(Resource.Id.PostDelayTime));

        public EditText IntervalTimeEditText => _intervalTimeEditText ?? (_intervalTimeEditText = View.FindViewById<EditText>(Resource.Id.IntervalTime));

        public EditText DurationTimeEditText => _durationTimeEditText ?? (_durationTimeEditText = View.FindViewById<EditText>(Resource.Id.DurationTime));

        public EditText MaxShotsEditText => _maxShotsEditText ?? (_maxShotsEditText = View.FindViewById<EditText>(Resource.Id.MaxShots));

        public EditText SliderStartPosEditText => _sliderStartPosEditText ?? (_sliderStartPosEditText = View.FindViewById<EditText>(Resource.Id.SliderStartPos));

        public EditText SliderStopPosEditText => _sliderStopPosEditText ?? (_sliderStopPosEditText = View.FindViewById<EditText>(Resource.Id.SliderStopPos));

        public EditText PanStartPosEditText => _panStartPosEditText ?? (_panStartPosEditText = View.FindViewById<EditText>(Resource.Id.PanStartPos));

        public EditText PanStopPosEditText => _panStopPosEditText ?? (_panStopPosEditText = View.FindViewById<EditText>(Resource.Id.PanStopPos));

        public EditText TiltStartPosEditText => _tiltStartPosEditText ?? (_tiltStartPosEditText = View.FindViewById<EditText>(Resource.Id.TiltStartPos));

        public EditText TiltStopPosEditText => _tiltStopPosEditText ?? (_tiltStopPosEditText = View.FindViewById<EditText>(Resource.Id.TiltStopPos));
    }
}
