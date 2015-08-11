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

            SetStartButton.Click += (o, e) => 
                {
                    var dlg = new JoystickViewFragment();
                    dlg.SetCommand("Closed", Vm.SetStartCommand);
                    dlg.ShowsDialog = true;
                    dlg.Show(FragmentManager, "dlg");

                    Console.WriteLine("Foo");
                };
            //SetStartButton.SetCommand("Click", Vm.SetStartCommand);

            SetStopButton.Click += (o, e) => 
                {
                    var dlg = new JoystickViewFragment();
                    dlg.SetCommand("Closed", Vm.SetStopCommand);
                    dlg.ShowsDialog = true;
                    dlg.Show(FragmentManager, "dlg");

                    Console.WriteLine("Foo");
                };
            //SetStopButton.SetCommand("Click", Vm.SetStopCommand);

            SwapStartStopButton.Click += (o, e) => {};
            SwapStartStopButton.SetCommand("Click", Vm.SwapStartStopCommand);

            StartProgramButton.Click += (o, e) => {};
            StartProgramButton.SetCommand("Click", Vm.StartProgramCommand);
            PauseProgramButton.Click += (o, e) => {};
            PauseProgramButton.SetCommand("Click", Vm.PauseProgramCommand);
            StopProgramButton.Click += (o, e) => {};
            StopProgramButton.SetCommand("Click", Vm.StopProgramCommand);
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
    }
}
    