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
using MDKControl.Droid.Activities;
using MDKControl.Droid.Widgets;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid.Fragments
{
    public class ModeSmsStatusViewFragment : DialogFragment
    {
        private Button _resumeButton;
        private Button _pauseButton;
        private Button _stopButton;

        public event EventHandler Resumed;
        public event EventHandler Paused;
        public event EventHandler Stoped;

        public static ModeSmsStatusViewFragment NewInstance() 
        {
            var args = new Bundle();

            var f = new ModeSmsStatusViewFragment();
            f.Arguments = args;
            f.ShowsDialog = true;

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ModeSmsStatusView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ResumeButton.Click += (o, e) => 
                { 
                    var handler = Resumed;
                    if (handler != null) handler(this, EventArgs.Empty);
                };
            PauseButton.Click += (o, e) => 
                { 
                    var handler = Paused;
                    if (handler != null) handler(this, EventArgs.Empty);
                };
            StopButton.Click += (o, e) => 
                { 
                    var handler = Stoped;
                    if (handler != null) handler(this, EventArgs.Empty);
                    Dismiss();
                };
        }

        public ModeSmsViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)Activity).Vm.ModeSmsViewModel;
            }
        }

        public Button ResumeButton
        {
            get
            {
                return _resumeButton
                    ?? (_resumeButton = View.FindViewById<Button>(Resource.Id.Resume));
            }
        }

        public Button PauseButton
        {
            get
            {
                return _pauseButton
                    ?? (_pauseButton = View.FindViewById<Button>(Resource.Id.Pause));
            }
        }

        public Button StopButton
        {
            get
            {
                return _stopButton
                    ?? (_stopButton = View.FindViewById<Button>(Resource.Id.Stop));
            }
        }
    }
}
    