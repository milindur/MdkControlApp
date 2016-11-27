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
    public class ModeAstroViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private readonly object _runStatusLock = new object();
        private MoCoBusRunStatus _prevRunStatus = MoCoBusRunStatus.Stopped;

        private Binding _sliderAxisRadioBinding;
        private Binding _panAxisRadioBinding;
        private Binding _tiltAxisRadioBinding;
        private Binding _mdkV5RadioBinding;
        private Binding _mdkV6RadioBinding;
        private Binding _nicOTiltRadioBinding;
        private Binding _otherMechanicsRadioBinding;
        private Binding _otherMechanicsGearReductionBinding;
        private Binding _northRadioBinding;
        private Binding _southRadioBinding;
        private Binding _siderealRadioBinding;
        private Binding _lunarRadioBinding;

        private Button _startProgramButton;
        private RadioButton _sliderAxisRadioButton;
        private RadioButton _panAxisRadioButton;
        private RadioButton _tiltAxisRadioButton;
        private RadioButton _mdkV5RadioButton;
        private RadioButton _mdkV6RadioButton;
        private RadioButton _nicOTiltRadioButton;
        private RadioButton _otherMechanicsRadioButton;
        private RadioButton _northRadioButton;
        private RadioButton _southRadioButton;
        private RadioButton _siderealRadioButton;
        private RadioButton _lunarRadioButton;
        private EditText _otherMechanicsGearReductionEditText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModeAstroView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            Vm.PropertyChanged += (o, e) => {};

            StartProgramButton.Click += (o, e) => 
                {
                    System.Diagnostics.Debug.WriteLine("ModeAstroViewFragment StartProgramButton Clicked");

                    var ft = FragmentManager.BeginTransaction();
                    ft.DisallowAddToBackStack();
                    var dlg = ModeAstroStatusViewFragment.NewInstance();
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

            System.Diagnostics.Debug.WriteLine("ModeAstroViewFragment OnResume");

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
                                    var dlg2 = ModeAstroStatusViewFragment.NewInstance();
                                    dlg2.Stoped += (oo, ee) => { };
                                    dlg2.Paused += (oo, ee) => { };
                                    dlg2.Resumed += (oo, ee) => { };
                                    dlg2.SetCommand("Stoped", Vm.StopProgramCommand);
                                    dlg2.SetCommand("Paused", Vm.PauseProgramCommand);
                                    dlg2.SetCommand("Resumed", Vm.ResumeProgramCommand);
                                    dlg2.Show(ft, Consts.DialogTag);
                                }
                                DeviceVm.StartUpdateTask();
                            }
                        }

                        _prevRunStatus = DeviceVm.RunStatus;
                    });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();

            _sliderAxisRadioBinding = this.SetBinding(() => SliderAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (SliderAxisRadioButton.Checked)
                    {
                        Vm.Motors = Motors.MotorSlider;
                    }
                });

            _panAxisRadioBinding = this.SetBinding(() => PanAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (PanAxisRadioButton.Checked)
                    {
                        Vm.Motors = Motors.MotorPan;
                    }
                });

            _tiltAxisRadioBinding = this.SetBinding(() => TiltAxisRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (TiltAxisRadioButton.Checked)
                    {
                        Vm.Motors = Motors.MotorTilt;
                    }
                });

            _mdkV5RadioBinding = this.SetBinding(() => MdkV5RadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (MdkV5RadioButton.Checked)
                    {
                        Vm.GearType = GearType.MdkV5;
                    }
                });

            _mdkV6RadioBinding = this.SetBinding(() => MdkV6RadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (MdkV6RadioButton.Checked)
                    {
                        Vm.GearType = GearType.MdkV6;
                    }
                });

            _nicOTiltRadioBinding = this.SetBinding(() => NicOTiltRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (NicOTiltRadioButton.Checked)
                    {
                        Vm.GearType = GearType.NicOTilt;
                    }
                });

            _otherMechanicsRadioBinding = this.SetBinding(() => OtherMechanicsRadioButton.Checked)
                .WhenSourceChanges(() =>
                {
                    if (OtherMechanicsRadioButton.Checked)
                    {
                        Vm.GearReduction = 60.0f;
                    }
                });

            _northRadioBinding = this.SetBinding(() => NorthRadioButton.Checked)
                .WhenSourceChanges(() =>
                    {
                        if (NorthRadioButton.Checked) Vm.Direction = AstroDirection.North;
                    });
            _southRadioBinding = this.SetBinding(() => SouthRadioButton.Checked)
                .WhenSourceChanges(() =>
                    {
                        if (SouthRadioButton.Checked) Vm.Direction = AstroDirection.South;
                    });

            _siderealRadioBinding = this.SetBinding(() => SiderealRadioButton.Checked)
                .WhenSourceChanges(() =>
                    {
                        if (SiderealRadioButton.Checked) Vm.Speed = AstroSpeed.Sidereal;
                    });
            _lunarRadioBinding = this.SetBinding(() => LunarRadioButton.Checked)
                .WhenSourceChanges(() =>
                    {
                        if (LunarRadioButton.Checked) Vm.Speed = AstroSpeed.Lunar;
                    });
        }

        public override void OnPause()
        {
            _sliderAxisRadioBinding?.Detach();
            _panAxisRadioBinding?.Detach();
            _tiltAxisRadioBinding?.Detach();
            _mdkV5RadioBinding?.Detach();
            _mdkV6RadioBinding?.Detach();
            _nicOTiltRadioBinding?.Detach();
            _otherMechanicsRadioBinding?.Detach();
            _otherMechanicsGearReductionBinding?.Detach();
            _northRadioBinding?.Detach();
            _southRadioBinding?.Detach();
            _siderealRadioBinding?.Detach();
            _lunarRadioBinding?.Detach();
            _runStatusBinding?.Detach();

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>(Consts.DialogTag);
            dlg?.DismissAllowingStateLoss();

            base.OnPause();
        }

        public ModeAstroViewModel Vm => ((DeviceViewActivity)_activity).Vm.ModeAstroViewModel;

        public DeviceViewModel DeviceVm => ((DeviceViewActivity)_activity).Vm;

        public Button StartProgramButton => _startProgramButton ?? (_startProgramButton = View.FindViewById<Button>(Resource.Id.StartProgram));

        public RadioButton SliderAxisRadioButton => _sliderAxisRadioButton ?? (_sliderAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.SliderAxisRadioButton));

        public RadioButton PanAxisRadioButton => _panAxisRadioButton ?? (_panAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.PanAxisRadioButton));

        public RadioButton TiltAxisRadioButton => _tiltAxisRadioButton ?? (_tiltAxisRadioButton = View.FindViewById<RadioButton>(Resource.Id.TiltAxisRadioButton));

        public RadioButton MdkV5RadioButton => _mdkV5RadioButton ?? (_mdkV5RadioButton = View.FindViewById<RadioButton>(Resource.Id.MdkV5RadioButton));

        public RadioButton MdkV6RadioButton => _mdkV6RadioButton ?? (_mdkV6RadioButton = View.FindViewById<RadioButton>(Resource.Id.MdkV6RadioButton));

        public RadioButton NicOTiltRadioButton => _nicOTiltRadioButton ?? (_nicOTiltRadioButton = View.FindViewById<RadioButton>(Resource.Id.NoTRadioButton));

        public RadioButton OtherMechanicsRadioButton => _otherMechanicsRadioButton ?? (_otherMechanicsRadioButton = View.FindViewById<RadioButton>(Resource.Id.OtherMechanicsRadioButton));

        public RadioButton NorthRadioButton => _northRadioButton ?? (_northRadioButton = View.FindViewById<RadioButton>(Resource.Id.NorthRadioButton));

        public RadioButton SouthRadioButton => _southRadioButton ?? (_southRadioButton = View.FindViewById<RadioButton>(Resource.Id.SouthRadioButton));

        public RadioButton SiderealRadioButton => _siderealRadioButton ?? (_siderealRadioButton = View.FindViewById<RadioButton>(Resource.Id.SiderealRadioButton));

        public RadioButton LunarRadioButton => _lunarRadioButton ?? (_lunarRadioButton = View.FindViewById<RadioButton>(Resource.Id.LunarRadioButton));
    }
}
