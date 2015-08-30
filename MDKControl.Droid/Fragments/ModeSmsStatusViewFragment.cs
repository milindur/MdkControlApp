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
using MDKControl.Core.Models;

namespace MDKControl.Droid.Fragments
{
    public class ModeSmsStatusViewFragment : DialogFragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private Binding _progressBarBinding;
        private Binding _elapsedTimeBinding;
        private Binding _elapsedShotsBinding;

        private Button _resumeButton;
        private Button _pauseButton;
        private Button _stopButton;

        private ProgressBar _progressBar;
        private EditText _elapsedTimeEditText;
        private EditText _elapsedShotsEditText;
        private EditText _remainingTimeEditText;
        private EditText _remainingShotsEditText;
        private EditText _overallTimeEditText;
        private EditText _overallShotsEditText;
        private EditText _videoLength24EditText;
        private EditText _videoLength25EditText;
        private EditText _videoLength30EditText;

        public event EventHandler Resumed;
        public event EventHandler Paused;
        public event EventHandler Stoped;

        public static ModeSmsStatusViewFragment NewInstance() 
        {
            var args = new Bundle();

            var f = new ModeSmsStatusViewFragment();
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
            Dialog.SetTitle("SMS");
            
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

            _progressBarBinding = this.SetBinding(() => Vm.Progress).WhenSourceChanges(() =>
                {
                    ProgressBar.Progress = (int)Vm.Progress;
                });
            _elapsedTimeBinding = this.SetBinding(() => Vm.ElapsedTime).WhenSourceChanges(() =>
                {
                    ElapsedTimeEditText.Text = string.Format("{0}:{1:00}m", Vm.ElapsedTime.Minutes, Vm.ElapsedTime.Seconds);
                });
            _elapsedShotsBinding = this.SetBinding(() => Vm.ElapsedShots).WhenSourceChanges(() =>
                {
                    ElapsedShotsEditText.Text = string.Format("{0}", Vm.ElapsedShots);
                });
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("OnRunStatusChanged 1");

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
            _progressBarBinding.Detach();
            _elapsedTimeBinding.Detach();
            _elapsedShotsBinding.Detach();
            _runStatusBinding.Detach();

            base.OnPause();
        }

        public ModeSmsViewModel Vm
        {
            get
            {
                return ((DeviceViewActivity)Activity).Vm.ModeSmsViewModel;
            }
        }

        public DeviceViewModel DeviceVm
        {
            get
            {
                return ((DeviceViewActivity)Activity).Vm;
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

        public ProgressBar ProgressBar
        {
            get
            {
                return _progressBar
                    ?? (_progressBar = View.FindViewById<ProgressBar>(Resource.Id.Progress));
            }
        }

        public EditText ElapsedTimeEditText
        {
            get
            {
                return _elapsedTimeEditText
                    ?? (_elapsedTimeEditText = View.FindViewById<EditText>(Resource.Id.ElapsedTime));
            }
        }

        public EditText ElapsedShotsEditText
        {
            get
            {
                return _elapsedShotsEditText
                    ?? (_elapsedShotsEditText = View.FindViewById<EditText>(Resource.Id.ElapsedShots));
            }
        }

        public EditText RemainingTimeEditText
        {
            get
            {
                return _remainingTimeEditText
                    ?? (_remainingTimeEditText = View.FindViewById<EditText>(Resource.Id.RemainingTime));
            }
        }

        public EditText RemainingShotsEditText
        {
            get
            {
                return _remainingShotsEditText
                    ?? (_remainingShotsEditText = View.FindViewById<EditText>(Resource.Id.RemainingShots));
            }
        }

        public EditText OverallTimeEditText
        {
            get
            {
                return _overallTimeEditText
                    ?? (_overallTimeEditText = View.FindViewById<EditText>(Resource.Id.ElapsedTime));
            }
        }

        public EditText OverallShotsEditText
        {
            get
            {
                return _overallShotsEditText
                    ?? (_overallShotsEditText = View.FindViewById<EditText>(Resource.Id.ElapsedShots));
            }
        }

        public EditText VideoLength24EditText
        {
            get
            {
                return _videoLength24EditText
                    ?? (_videoLength24EditText = View.FindViewById<EditText>(Resource.Id.VideoLength24));
            }
        }

        public EditText VideoLength25EditText
        {
            get
            {
                return _videoLength25EditText
                    ?? (_videoLength25EditText = View.FindViewById<EditText>(Resource.Id.VideoLength25));
            }
        }

        public EditText VideoLength30EditText
        {
            get
            {
                return _videoLength30EditText
                    ?? (_videoLength30EditText = View.FindViewById<EditText>(Resource.Id.VideoLength30));
            }
        }
    }
}
    