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
    public class ModeAstroStatusViewFragment : DialogFragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;

        private Button _resumeButton;
        private Button _pauseButton;
        private Button _stopButton;

        public event EventHandler Resumed;
        public event EventHandler Paused;
        public event EventHandler Stoped;

        public static ModeAstroStatusViewFragment NewInstance() 
        {
            var args = new Bundle();

            var f = new ModeAstroStatusViewFragment();
            f.Arguments = args;
            f.ShowsDialog = true;
            f.Cancelable = false;

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Astro-Tracking");

            return inflater.Inflate(Resource.Layout.ModeAstroStatusView, container, false);
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

            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("ModeAstroStatusViewFragment OnRunStatusChanged 1");

                    ResumeButton.Enabled = false;
                    PauseButton.Enabled = false;
                    StopButton.Enabled = true;

                    if (DeviceVm.RunStatus == MoCoBusRunStatus.Running)
                    {
                        PauseButton.Enabled = true;
                    }
                    else if (DeviceVm.RunStatus == MoCoBusRunStatus.Paused)
                    {
                        ResumeButton.Enabled = true;
                    }
                });
        }

        public override void OnPause()
        {
            _runStatusBinding?.Detach();

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
    