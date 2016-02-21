using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;
using MDKControl.Core.Models;
using GalaSoft.MvvmLight.Helpers;

namespace MDKControl.iOS
{
	partial class ModeAstroStatusViewController : UIViewController, INavigationTarget
	{
        private Binding _runStatusBinding;

        public ModeAstroStatusViewController (IntPtr handle) : base (handle)
		{
		}

        public object NavigationParameter { get; set; }

        public ModeAstroViewModel Vm { get; private set; }
        public DeviceViewModel DeviceVm { get { return Vm.DeviceViewModel; } }

        public override void ViewDidLoad()
        {
            System.Diagnostics.Debug.WriteLine("ModeAstroStatusViewController ViewDidLoad");

            Vm = (ModeAstroViewModel)NavigationParameter;

            CancelButton.Clicked += (sender, e) => 
                {
                    PauseResumeButton.Enabled = false;
                    CancelButton.Enabled = false;

                    Vm.StopProgramCommand.Execute(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();
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
                });

            base.ViewDidLoad();
        }
	}
}
