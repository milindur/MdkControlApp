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
    public class ModePanoStatusViewFragment : DialogFragment
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
        private Binding _overallColsBinding;
        private Binding _overallRowsBinding;

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
        private EditText _overallColsEditText;
        private EditText _overallRowsEditText;

        public event EventHandler Resumed;
        public event EventHandler Paused;
        public event EventHandler Stoped;

        public static ModePanoStatusViewFragment NewInstance()
        {
            var args = new Bundle();

            var f = new ModePanoStatusViewFragment
            {
                Arguments = args,
                ShowsDialog = true,
                Cancelable = false
            };

            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Panorama");

            return inflater.Inflate(Resource.Layout.ModePanoStatusView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
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
                    ElapsedTimeEditText.Text = $"{(int)Vm.ElapsedTime.TotalMinutes}:{Vm.ElapsedTime.Seconds:00}m";
                });
            _elapsedShotsBinding = this.SetBinding(() => Vm.ElapsedShots).WhenSourceChanges(() =>
                {
                    ElapsedShotsEditText.Text = $"{Vm.ElapsedShots}";
                });
            _remainingTimeBinding = this.SetBinding(() => Vm.RemainingTime).WhenSourceChanges(() =>
                {
                    RemainingTimeEditText.Text = $"{(int)Vm.RemainingTime.TotalMinutes}:{Vm.RemainingTime.Seconds:00}m";
                });
            _remainingShotsBinding = this.SetBinding(() => Vm.RemainingShots).WhenSourceChanges(() =>
                {
                    RemainingShotsEditText.Text = $"{Vm.RemainingShots}";
                });
            _overallTimeBinding = this.SetBinding(() => Vm.OverallTime).WhenSourceChanges(() =>
                {
                    OverallTimeEditText.Text = $"{(int)Vm.OverallTime.TotalMinutes}:{Vm.OverallTime.Seconds:00}m";
                });
            _overallShotsBinding = this.SetBinding(() => Vm.OverallShots).WhenSourceChanges(() =>
                {
                    OverallShotsEditText.Text = $"{Vm.OverallShots}";
                });
            _overallColsBinding = this.SetBinding(() => Vm.OverallCols).WhenSourceChanges(() =>
                {
                    OverallColsEditText.Text = $"{Vm.OverallCols}";
                });
            _overallRowsBinding = this.SetBinding(() => Vm.OverallRows).WhenSourceChanges(() =>
                {
                    OverallRowsEditText.Text = $"{Vm.OverallRows}";
                });
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    System.Diagnostics.Debug.WriteLine("ModePanoStatusViewFragment OnRunStatusChanged 1");

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
            _progressBarBinding?.Detach();
            _elapsedTimeBinding?.Detach();
            _elapsedShotsBinding?.Detach();
            _remainingTimeBinding?.Detach();
            _remainingShotsBinding?.Detach();
            _overallTimeBinding?.Detach();
            _overallShotsBinding?.Detach();
            _overallColsBinding?.Detach();
            _overallRowsBinding?.Detach();
            _runStatusBinding?.Detach();

            base.OnPause();
        }

        public ModePanoViewModel Vm => ((DeviceViewActivity)_activity).Vm.ModePanoViewModel;

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

        public EditText OverallColsEditText => _overallColsEditText ?? (_overallColsEditText = View.FindViewById<EditText>(Resource.Id.OverallCols));

        public EditText OverallRowsEditText => _overallRowsEditText ?? (_overallRowsEditText = View.FindViewById<EditText>(Resource.Id.OverallRows));
    }
}
