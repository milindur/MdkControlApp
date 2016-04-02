using System;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.iOS
{
    internal partial class ModeSmsStatusViewController : SubDeviceViewControllerBase, INavigationTarget
    {
        private Binding _runStatusBinding;
        private Binding _progressBarBinding;
        private Binding _elapsedTimeBinding;
        private Binding _remainingTimeBinding;
        private Binding _overallTimeBinding;
        private Binding _elapsedShotsBinding;
        private Binding _remainingShotsBinding;
        private Binding _overallShotsBinding;
        private Binding _videoLength24Binding;
        private Binding _videoLength25Binding;
        private Binding _videoLength30Binding;

        private bool _canceled;

        public ModeSmsStatusViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.ShootMoveShoot, AppDelegate.ModeSmsStatusViewKey)
        {
        }

        public object NavigationParameter { get; set; }

        public ModeSmsViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm => Vm.DeviceViewModel;

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewController ViewDidLoad");

            Vm = (ModeSmsViewModel)NavigationParameter;

            CancelButton.Clicked += (sender, e) =>
            {
                PauseResumeButton.Enabled = false;
                CancelButton.Enabled = false;

                _canceled = true;
                Vm.StopProgramCommand.Execute(null);
            };

            PauseResumeButton.Clicked += (sender, e) =>
            {
                PauseResumeButton.Enabled = false;
                CancelButton.Enabled = false;

                switch (DeviceVm.RunStatus)
                {
                    case MoCoBusRunStatus.Paused:
                        Vm.StartProgramCommand.Execute(null);
                        break;
                    case MoCoBusRunStatus.Running:
                        Vm.PauseProgramCommand.Execute(null);
                        break;
                }
            };

            SetupBindings();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewController ViewWillAppear");

            _canceled = false;

            SetupBindings();
            
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModeSmsStatusViewController ViewWillDisappear");

            DetachBindings();

            base.ViewWillDisappear(animated);
        }

        protected override void SetupBindings()
        {
            DetachBindings();

            base.SetupBindings();

            _elapsedTimeBinding = this.SetBinding(() => Vm.ElapsedTime).WhenSourceChanges(() =>
                {
                    ElapsedTimeValueLabel.Text = $"{(int) Vm.ElapsedTime.TotalMinutes}:{Vm.ElapsedTime.Seconds:00}m";
                });
            _elapsedShotsBinding = this.SetBinding(() => Vm.ElapsedShots).WhenSourceChanges(() =>
                {
                    ElapsedShotsValueLabel.Text = $"{Vm.ElapsedShots}";
                });
            _remainingTimeBinding = this.SetBinding(() => Vm.RemainingTime).WhenSourceChanges(() =>
                {
                    RemainingTimeValueLabel.Text = $"{(int) Vm.RemainingTime.TotalMinutes}:{Vm.RemainingTime.Seconds:00}m";
                });
            _remainingShotsBinding = this.SetBinding(() => Vm.RemainingShots).WhenSourceChanges(() =>
                {
                    RemainingShotsValueLabel.Text = $"{Vm.RemainingShots}";
                });
            _overallTimeBinding = this.SetBinding(() => Vm.DurationTime).WhenSourceChanges(() =>
                {
                    OverallTimeValueLabel.Text = $"{(int) (Vm.DurationTime/60)}:{(int) Vm.DurationTime%60:00}m";
                });
            _overallShotsBinding = this.SetBinding(() => Vm.MaxShots).WhenSourceChanges(() =>
                {
                    OverallShotsValueLabel.Text = $"{Vm.MaxShots}";
                });
            _videoLength24Binding = this.SetBinding(() => Vm.VideoLength24).WhenSourceChanges(() =>
                {
                    VideoLength24ValueLabel.Text = $"{(int) Vm.VideoLength24.TotalMinutes}:{Vm.VideoLength24.Seconds:00}m";
                });
            _videoLength25Binding = this.SetBinding(() => Vm.VideoLength25).WhenSourceChanges(() =>
                {
                    VideoLength25ValueLabel.Text = $"{(int) Vm.VideoLength25.TotalMinutes}:{Vm.VideoLength25.Seconds:00}m";
                });
            _videoLength30Binding = this.SetBinding(() => Vm.VideoLength30).WhenSourceChanges(() =>
                {
                    VideoLength30ValueLabel.Text = $"{(int) Vm.VideoLength30.TotalMinutes}:{Vm.VideoLength30.Seconds:00}m";
                });

            _progressBarBinding = this.SetBinding(() => Vm.Progress).WhenSourceChanges(() =>
                {
                    ProgressBar.Progress = Vm.Progress / 100f;
                });
            
            _runStatusBinding = this.SetBinding(() => DeviceVm.RunStatus).WhenSourceChanges(() =>
                {
                    PauseResumeButton.Enabled = true;
                    CancelButton.Enabled = true;
                    if (DeviceVm.RunStatus == MoCoBusRunStatus.Running)
                    {
                        PauseResumeButton.Title = "Pause";
                    }
                    else if (DeviceVm.RunStatus == MoCoBusRunStatus.Paused)
                    {
                        PauseResumeButton.Title = "Resume";
                    }
                    else if (DeviceVm.RunStatus == MoCoBusRunStatus.Stopped)
                    {
                        PauseResumeButton.Title = "Finished";
                        if (_canceled)
                            ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
                    }
                });
        }

        protected override void DetachBindings()
        {
            _elapsedTimeBinding?.Detach();
            _elapsedTimeBinding = null;

            _remainingTimeBinding?.Detach();
            _remainingTimeBinding = null;

            _overallTimeBinding?.Detach();
            _overallTimeBinding = null;

            _elapsedShotsBinding?.Detach();
            _elapsedShotsBinding = null;

            _remainingShotsBinding?.Detach();
            _remainingShotsBinding = null;

            _overallShotsBinding?.Detach();
            _overallShotsBinding = null;

            _videoLength24Binding?.Detach();
            _videoLength24Binding = null;

            _videoLength25Binding?.Detach();
            _videoLength25Binding = null;

            _videoLength30Binding?.Detach();
            _videoLength30Binding = null;

            _progressBarBinding?.Detach();
            _progressBarBinding = null;

            _runStatusBinding?.Detach();
            _runStatusBinding = null;

            base.DetachBindings();
        }
    }
}
