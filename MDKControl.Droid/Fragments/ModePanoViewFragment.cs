using System;
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
    public class ModePanoViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private object _runStatusLock = new object();
        private MoCoBusRunStatus _prevRunStatus = MoCoBusRunStatus.Stopped;

        private Binding _exposureTimeBinding;
        private Binding _delayTimeBinding;
        
        private Binding _panStartPosBinding;
        private Binding _panStopPosBinding;
        private Binding _panSizeBinding;
        private Binding _tiltStartPosBinding;
        private Binding _tiltStopPosBinding;
        private Binding _tiltSizeBinding;

        private Button _setStartButton;
        private Button _setStopButton;
        private Button _swapStartStopButton;
        private Button _setRefStartButton;
        private Button _setRefStopButton;
        private Button _startProgramButton;

        private EditText _exposureTimeEditText;
        private EditText _delayTimeEditText;

        private EditText _panStartPosEditText;
        private EditText _panStopPosEditText;
        private EditText _panSizeEditText;
        private EditText _tiltStartPosEditText;
        private EditText _tiltStopPosEditText;
        private EditText _tiltSizeEditText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModePanoView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            Vm.PropertyChanged += (o, e) => {};

            ExposureTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Exposure", Vm.ExposureTime);
                    dlg.Closed += (oo, ee) => { Vm.ExposureTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            DelayTimeEditText.Click += (o, e) => 
                {
                    var dlg = TimeViewFragment.NewInstance("Delay", Vm.DelayTime);
                    dlg.Closed += (oo, ee) => { Vm.DelayTime = ee; };
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            SetStartButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Start");
                    dlg.SetCommand("Closed", Vm.SetStartCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };
            SetStopButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Stop");
                    dlg.SetCommand("Closed", Vm.SetStopCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            SwapStartStopButton.Click += (o, e) => {};
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);

            SetRefStartButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Fov Start");
                    dlg.SetCommand("Closed", Vm.SetRefStartCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };
            SetRefStopButton.Click += (o, e) => 
                {
                    var dlg = JoystickViewFragment.NewInstance("Set Fov Stop");
                    dlg.SetCommand("Closed", Vm.SetRefStopCommand);
                    dlg.Show(FragmentManager, Consts.DialogTag);
                };

            StartProgramButton.Click += (o, e) => 
                {
                    System.Diagnostics.Debug.WriteLine("ModePanoViewFragment StartProgramButton Clicked");

                    var ft = FragmentManager.BeginTransaction();
                    ft.DisallowAddToBackStack();
                    var dlg = ModePanoStatusViewFragment.NewInstance();
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

            System.Diagnostics.Debug.WriteLine("ModePanoViewFragment OnResume");

            _prevRunStatus = MoCoBusRunStatus.Stopped;
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus)
                .WhenSourceChanges(() =>
                    {
                        lock (_runStatusLock)
                        {
                            if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped && DeviceVm.RunStatus != _prevRunStatus && !DeviceVm.IsUpdateTaskRunning)
                            {
                                var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
                                if (dlg == null)
                                {
                                    var ft = FragmentManager.BeginTransaction();
                                    ft.DisallowAddToBackStack();
                                    dlg = ModePanoStatusViewFragment.NewInstance();
                                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                                    dlg.SetCommand("Resumed", Vm.StartProgramCommand);
                                    dlg.Show(ft, Consts.DialogTag);
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
                        ExposureTimeEditText.Text = string.Format("{0:F1}s", Vm.ExposureTime); 
                    });
            _exposureTimeBinding.ForceUpdateValueFromSourceToTarget();

            _delayTimeBinding = this.SetBinding(() => Vm.DelayTime)
                .WhenSourceChanges(() =>
                    { 
                        DelayTimeEditText.Text = string.Format("{0:F1}s", Vm.DelayTime); 
                    });
            _delayTimeBinding.ForceUpdateValueFromSourceToTarget();
        
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

            _panSizeBinding = this.SetBinding(() => Vm.PanSize)
                .WhenSourceChanges(() =>
                    {
                        PanSizeEditText.Text = string.Format("{0:F1}°", (double)Vm.PanSize / (190 * 200 * 16) * 360);
                    });
            _panSizeBinding.ForceUpdateValueFromSourceToTarget();

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
        
            _tiltSizeBinding = this.SetBinding(() => Vm.TiltSize)
                .WhenSourceChanges(() =>
                    {
                        TiltSizeEditText.Text = string.Format("{0:F1}°", (double)Vm.TiltSize / (190 * 200 * 16) * 360);
                    });
            _tiltSizeBinding.ForceUpdateValueFromSourceToTarget();
        }

        public override void OnPause()
        {
            _runStatusBinding?.Detach();
            _exposureTimeBinding?.Detach();
            _delayTimeBinding?.Detach();

            _panStartPosBinding?.Detach();
            _panStopPosBinding?.Detach();
            _panSizeBinding?.Detach();
            _tiltStartPosBinding?.Detach();
            _tiltStopPosBinding?.Detach();
            _tiltSizeBinding?.Detach();

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            if (dlg != null)
            {
                dlg.DismissAllowingStateLoss();
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

        public EditText PanSizeEditText
        {
            get
            {
                return _panSizeEditText
                    ?? (_panSizeEditText = View.FindViewById<EditText>(Resource.Id.PanSize));
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

        public EditText TiltSizeEditText
        {
            get
            {
                return _tiltSizeEditText
                    ?? (_tiltSizeEditText = View.FindViewById<EditText>(Resource.Id.TiltSize));
            }
        }
    }
}
    