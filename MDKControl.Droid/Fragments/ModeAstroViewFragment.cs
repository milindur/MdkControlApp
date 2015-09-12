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
    public class ModeAstroViewFragment : Fragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;

        private Button _startProgramNorthButton;
        private Button _startProgramSouthButton;

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

            StartProgramNorthButton.Click += (o, e) => 
                {  
                    var dlg = ModeAstroStatusViewFragment.NewInstance();
                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                    dlg.SetCommand("Resumed", Vm.ResumeProgramCommand);
                    dlg.Show(FragmentManager, "statusDlg");
                };
            StartProgramNorthButton.SetCommand("Click", Vm.StartProgramNorthCommand);

            StartProgramSouthButton.Click += (o, e) => 
                {  
                    var dlg = ModeAstroStatusViewFragment.NewInstance();
                    dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                    dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                    dlg.SetCommand("Resumed", Vm.ResumeProgramCommand);
                    dlg.Show(FragmentManager, "statusDlg");
                };
            StartProgramSouthButton.SetCommand("Click", Vm.StartProgramSouthCommand);
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
                                dlg = ModeAstroStatusViewFragment.NewInstance();
                                dlg.SetCommand("Stoped", Vm.StopProgramCommand);
                                dlg.SetCommand("Paused", Vm.PauseProgramCommand);
                                dlg.SetCommand("Resumed", Vm.ResumeProgramCommand);
                                dlg.Show(FragmentManager, "statusDlg");
                                DeviceVm.StartUpdateTask();
                            }
                        }
                    });
        }

        public override void OnPause()
        {
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

        public Button StartProgramNorthButton
        {
            get
            {
                return _startProgramNorthButton
                    ?? (_startProgramNorthButton = View.FindViewById<Button>(Resource.Id.StartProgramNorth));
            }
        }

        public Button StartProgramSouthButton
        {
            get
            {
                return _startProgramSouthButton
                    ?? (_startProgramSouthButton = View.FindViewById<Button>(Resource.Id.StartProgramSouth));
            }
        }
    }
}
