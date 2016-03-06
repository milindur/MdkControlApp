using System;
using System.CodeDom.Compiler;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.Models;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using UIKit;

namespace MDKControl.iOS
{
    partial class ModePanoStatusViewController : SubDeviceViewControllerBase, INavigationTarget
    {
        private Binding _runStatusBinding;
        private Binding _progressBarBinding;
        private bool _canceled = false;

        public ModePanoStatusViewController(IntPtr handle)
            : base(handle, MoCoBusProgramMode.Panorama, AppDelegate.ModePanoStatusViewKey)
        {
        }

        public object NavigationParameter { get; set; }

        public ModePanoViewModel Vm { get; private set; }

        public override DeviceViewModel DeviceVm
        {
            get
            {
                return Vm.DeviceViewModel;
            }
        }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModePanoStatusViewController ViewDidLoad");

            Vm = (ModePanoViewModel)NavigationParameter;

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
            System.Diagnostics.Debug.WriteLine("ModePanoStatusViewController ViewWillAppear");

            _canceled = false;

            SetupBindings();
            
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            System.Diagnostics.Debug.WriteLine("ModePanoStatusViewController ViewWillDisappear");

            DetachBindings();

            base.ViewWillDisappear(animated);
        }

        protected override void SetupBindings()
        {
            DetachBindings();

            base.SetupBindings();

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
            _progressBarBinding?.Detach();
            _progressBarBinding = null;

            _runStatusBinding?.Detach();
            _runStatusBinding = null;

            base.DetachBindings();
        }
    }
}
