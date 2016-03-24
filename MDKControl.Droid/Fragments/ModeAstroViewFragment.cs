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
    public class ModeAstroViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private Binding _northRadioBinding;
        private Binding _southRadioBinding;
        private Binding _siderealRadioBinding;
        private Binding _lunarRadioBinding;

        private Button _startProgramButton;
        private RadioButton _northRadioButton;
        private RadioButton _southRadioButton;
        private RadioButton _siderealRadioButton;
        private RadioButton _lunarRadioButton;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModeAstroView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            StartProgramButton.Click += (o, e) => 
                {  
                    var dlg = ModeAstroStatusViewFragment.NewInstance();
                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                    dlg.SetCommand("Resumed", Vm.ResumeProgramCommand);
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

            System.Diagnostics.Debug.WriteLine("ModeAstroViewFragment OnResume");

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

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus)
                .WhenSourceChanges(() =>
                    {
                        if (DeviceVm.RunStatus != MoCoBusRunStatus.Stopped)
                        {
                            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>("statusDlg");
                            if (dlg == null)
                            {
                                dlg = ModeAstroStatusViewFragment.NewInstance();
                                dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                                dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                                dlg.SetCommand("Resumed", Vm.ResumeProgramCommand);
                                dlg.Show(FragmentManager, "statusDlg");
                            }

                            DeviceVm.StartUpdateTask();
                        }
                    });
            _runStatusBinding.ForceUpdateValueFromSourceToTarget();
        }

        public override void OnPause()
        {
            _northRadioBinding.Detach();
            _southRadioBinding.Detach();
            _siderealRadioBinding.Detach();
            _lunarRadioBinding.Detach();
            _runStatusBinding.Detach();

            var dlg = FragmentManager.FindFragmentByTag<DialogFragment>("statusDlg");
            if (dlg != null)
            {
                dlg.Dismiss();
            }

            base.OnPause();
        }

        public ModeAstroViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)_activity).Vm.ModeAstroViewModel;
            }
        }

        public DeviceViewModel DeviceVm
        {
            get
            {
                return ((DeviceViewActivity)_activity).Vm;
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

        public RadioButton NorthRadioButton
        {
            get
            {
                return _northRadioButton
                    ?? (_northRadioButton = View.FindViewById<RadioButton>(Resource.Id.NorthRadioButton));
            }
        }

        public RadioButton SouthRadioButton
        {
            get
            {
                return _southRadioButton
                    ?? (_southRadioButton = View.FindViewById<RadioButton>(Resource.Id.SouthRadioButton));
            }
        }

        public RadioButton SiderealRadioButton
        {
            get
            {
                return _siderealRadioButton
                    ?? (_siderealRadioButton = View.FindViewById<RadioButton>(Resource.Id.SiderealRadioButton));
            }
        }

        public RadioButton LunarRadioButton
        {
            get
            {
                return _lunarRadioButton
                    ?? (_lunarRadioButton = View.FindViewById<RadioButton>(Resource.Id.LunarRadioButton));
            }
        }
    }
}
