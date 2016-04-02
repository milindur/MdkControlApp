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
    public class ModeSmsStatusViewFragment : DialogFragment
    {
        private Activity _activity;

        private Binding _runStatusBinding;
        private Binding _progressBarBinding;
        private Binding _elapsedTimeBinding;
        private Binding _elapsedShotsBinding;
        private Binding _remainingTimeBinding;
        private Binding _remainingShotsBinding;
        private Binding _overallTimeBinding;
        private Binding _overallShotsBinding;
        private Binding _videoLength24Binding;
        private Binding _videoLength25Binding;
        private Binding _videoLength30Binding;

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
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment NewInstance");

            var args = new Bundle();

            var f = new ModeSmsStatusViewFragment
            {
                Arguments = args,
                ShowsDialog = true,
                Cancelable = false
            };

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnCreate");

            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnCreateView");

            Dialog.SetTitle("SMS");
            
            return inflater.Inflate(Resource.Layout.ModeSmsStatusView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnActivityCreated");

            base.OnActivityCreated(savedInstanceState);

            ResumeButton.Click += (o, e) => 
                { 
                    Resumed?.Invoke(this, EventArgs.Empty);
                };
            PauseButton.Click += (o, e) => 
                { 
                    Paused?.Invoke(this, EventArgs.Empty);
                };
            StopButton.Click += (o, e) => 
                { 
                    Stoped?.Invoke(this, EventArgs.Empty);
                    Dismiss();
                };
        }

        public override void OnAttach(Activity activity)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnAttach");

            base.OnAttach(activity);

            _activity = activity;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnDestroy");

            base.OnDestroy();

            _activity = null;
        }

        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnResume");

            base.OnResume();

            _progressBarBinding = this.SetBinding(() => Vm.Progress).WhenSourceChanges(() =>
                {
                    ProgressBar.Progress = (int)Vm.Progress;
                });
            _elapsedTimeBinding = this.SetBinding(() => Vm.ElapsedTime).WhenSourceChanges(() =>
                {
                    ElapsedTimeEditText.Text = $"{(int) Vm.ElapsedTime.TotalMinutes}:{Vm.ElapsedTime.Seconds:00}m";
                });
            _elapsedShotsBinding = this.SetBinding(() => Vm.ElapsedShots).WhenSourceChanges(() =>
                {
                    ElapsedShotsEditText.Text = $"{Vm.ElapsedShots}";
                });
            _remainingTimeBinding = this.SetBinding(() => Vm.RemainingTime).WhenSourceChanges(() =>
                {
                    RemainingTimeEditText.Text = $"{(int) Vm.RemainingTime.TotalMinutes}:{Vm.RemainingTime.Seconds:00}m";
                });
            _remainingShotsBinding = this.SetBinding(() => Vm.RemainingShots).WhenSourceChanges(() =>
                {
                    RemainingShotsEditText.Text = $"{Vm.RemainingShots}";
                });
            _overallTimeBinding = this.SetBinding(() => Vm.DurationTime).WhenSourceChanges(() =>
                {
                    OverallTimeEditText.Text = $"{(int) (Vm.DurationTime/60)}:{(int) Vm.DurationTime%60:00}m";
                });
            _overallShotsBinding = this.SetBinding(() => Vm.MaxShots).WhenSourceChanges(() =>
                {
                    OverallShotsEditText.Text = $"{Vm.MaxShots}";
                });
            _videoLength24Binding = this.SetBinding(() => Vm.VideoLength24).WhenSourceChanges(() =>
                {
                    VideoLength24EditText.Text = $"{(int) Vm.VideoLength24.TotalMinutes}:{Vm.VideoLength24.Seconds:00}m";
                });
            _videoLength25Binding = this.SetBinding(() => Vm.VideoLength25).WhenSourceChanges(() =>
                {
                    VideoLength25EditText.Text = $"{(int) Vm.VideoLength25.TotalMinutes}:{Vm.VideoLength25.Seconds:00}m";
                });
            _videoLength30Binding = this.SetBinding(() => Vm.VideoLength30).WhenSourceChanges(() =>
                {
                    VideoLength30EditText.Text = $"{(int) Vm.VideoLength30.TotalMinutes}:{Vm.VideoLength30.Seconds:00}m";
                });
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnRunStatusChanged 1");

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
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewFragment OnPause");

            _progressBarBinding?.Detach();
            _elapsedTimeBinding?.Detach();
            _elapsedShotsBinding?.Detach();
            _remainingTimeBinding?.Detach();
            _remainingShotsBinding?.Detach();
            _overallTimeBinding?.Detach();
            _overallShotsBinding?.Detach();
            _videoLength24Binding?.Detach();
            _videoLength25Binding?.Detach();
            _videoLength30Binding?.Detach();
            _runStatusBinding?.Detach();

            base.OnPause();
        }

        public ModeSmsViewModel Vm => ((DeviceViewActivity)_activity).Vm.ModeSmsViewModel;

        public DeviceViewModel DeviceVm => ((DeviceViewActivity)_activity).Vm;

        public Button ResumeButton => _resumeButton ?? (_resumeButton = View.FindViewById<Button>(Resource.Id.Resume));

        public Button PauseButton => _pauseButton ?? (_pauseButton = View.FindViewById<Button>(Resource.Id.Pause));

        public Button StopButton => _stopButton ?? (_stopButton = View.FindViewById<Button>(Resource.Id.Stop));

        public ProgressBar ProgressBar => _progressBar ?? (_progressBar = View.FindViewById<ProgressBar>(Resource.Id.Progress));

        public EditText ElapsedTimeEditText => _elapsedTimeEditText ?? (_elapsedTimeEditText = View.FindViewById<EditText>(Resource.Id.ElapsedTime));

        public EditText ElapsedShotsEditText => _elapsedShotsEditText ?? (_elapsedShotsEditText = View.FindViewById<EditText>(Resource.Id.ElapsedShots));

        public EditText RemainingTimeEditText => _remainingTimeEditText ?? (_remainingTimeEditText = View.FindViewById<EditText>(Resource.Id.RemainingTime));

        public EditText RemainingShotsEditText => _remainingShotsEditText ?? (_remainingShotsEditText = View.FindViewById<EditText>(Resource.Id.RemainingShots));

        public EditText OverallTimeEditText => _overallTimeEditText ?? (_overallTimeEditText = View.FindViewById<EditText>(Resource.Id.OverallTime));

        public EditText OverallShotsEditText => _overallShotsEditText ?? (_overallShotsEditText = View.FindViewById<EditText>(Resource.Id.OverallShots));

        public EditText VideoLength24EditText => _videoLength24EditText ?? (_videoLength24EditText = View.FindViewById<EditText>(Resource.Id.VideoLength24));

        public EditText VideoLength25EditText => _videoLength25EditText ?? (_videoLength25EditText = View.FindViewById<EditText>(Resource.Id.VideoLength25));

        public EditText VideoLength30EditText => _videoLength30EditText ?? (_videoLength30EditText = View.FindViewById<EditText>(Resource.Id.VideoLength30));
    }
}
    